using FastKart.Auth;
using FastKart.Exceptions;
using FastKart.Helpers;
using FastKart.Models;
using FastKart.Models.Data;
using FastKart.Models.DTOs;
using FastKart.Models.Requests;
using FastKart.Models.Responses;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace FastKart.Controllers
{

    [Route("api/[controller]")]
    //[ApiController]
    public partial class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly JwtHelper jwtHelper;
        private readonly JwtOptions jwtOptions;

        public AuthController(AppDbContext context, JwtHelper jwtHelper, JwtOptions jwtOptions)
        {
            _context = context;
            this.jwtHelper = jwtHelper;
            this.jwtOptions = jwtOptions;
        }

        public async Task<ActionResult<ApiResponse>>? Validate<T>(IValidator<T> validator, T modelToValidate)
        {
            var validationResult = await validator.ValidateAsync(modelToValidate);
            if (!validationResult.IsValid)
            {
                var errorDetails = validationResult.Errors
                    .GroupBy(error => error.PropertyName)
                    .ToDictionary(
                        group => group.Key,
                        group => group
                            .Select(error => error.ErrorMessage)
                            .ToArray());
                return BadRequest(new ApiResponse()
                {
                    Success = false,
                    Data = false,
                    Error = new ApiError()
                    {
                        Code = ApiErrorCodes.ValidationFailed,
                        Message = "validation error message",
                        Details = errorDetails!,
                    }
                });
            }

            return null!;
        }

        [HttpPost("register")]
        public async Task<ActionResult<ApiResponse>> Register(RegisterModel registerModel, [FromServices] IValidator<RegisterModel> validator) // register as client
        {
            

            var clientRole = await _context.Roles.FindAsync("Client") ?? throw new DefaultRoleDoesntExistException("role client doesn't exist!");

            var userExists = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == registerModel.Email) != null;
            if (userExists)
            {
                return BadRequest(new ApiResponse()
                {
                    Success = false,
                    Data = null!,
                    Error = new ApiError() { Code = ApiErrorCodes.EmailAlreadyExists, Message = "Email Already Exists"}
                });
            }

            var user = new AppUser()
            {
                Name = registerModel.Name,
                Email = registerModel.Email,
                Phone = registerModel.Phone,
                PasswordHash = PasswordHasher.CreatePasswordHash(registerModel.Password),
                Role = clientRole,
            };

            await _context.AddAsync(user);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse() { Success= true, Data = new UserWithRoleNameDTO(user)});
        }

        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse>> Login(LoginModel loginModel, [FromServices] IValidator<LoginModel> validator)
        {
            if(await Validate(validator, loginModel)! is ActionResult<ApiResponse> response) // validation reduction attempt
            {
                return response;
            }

            var user = await _context.Users.AsNoTracking().Include(u => u.Role).FirstOrDefaultAsync(u => u.Email == loginModel.Email);

            if (user is null)
            {
                return BadRequest("user doesn't exist");
            }

            if (!PasswordHasher.VerifyPassword(loginModel.Password, user.PasswordHash))
            {
                return BadRequest("password mismatch");
            }


            var token = await jwtHelper.GenerateAccessTokenAsync(user); // make it properly working using result pattern
            var refeshToken = jwtHelper.GenerateRandomRefreshToken();

            var refreshTokenHash = jwtHelper.Hash(refeshToken);
            var refreshTokenModel = new RefreshToken()
            {
                UserId = user.Id,
                RefreshTokenHash = refreshTokenHash,
                ExpiresAt = DateTime.UtcNow.AddMinutes(jwtOptions.RefreshTokenLifeTime)
            };

            _context.Add(refreshTokenModel);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse() {
                Success = true,
                Data = new { user = new UserWithRoleNameDTO(user), token, refeshToken }
            });
        }

        [HttpPost("logout")]
        public async Task<ActionResult<ApiResponse>> Logout(string refreshToken)
        {
            // blocking the refresh token
            var refreshTokenModel = await _context.RefreshTokens.FindAsync(jwtHelper.Hash(refreshToken));
            if (refreshTokenModel == null)
            {
                return NotFound(new ApiResponse()
                {
                    Success = false,
                    Data = null!, // TODO : fix all null! occurences
                    Error = new ApiError()
                    {
                        Code = ApiErrorCodes.NotFound,
                        Message = "refresh token doesn't exist",
                    }
                });
            }

            refreshTokenModel.Blocked = true;
            await _context.SaveChangesAsync();
            return Ok(new ApiResponse()
            {
                Success = true,
                Data = "Refresh token blocked successfuly"
            });
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<ApiResponse>> RefreshToken(string refreshToken)
        {
            var refreshTokenModel = await _context.RefreshTokens.FindAsync(jwtHelper.Hash(refreshToken));
            if (refreshTokenModel == null)
            {
                return NotFound(new ApiResponse()
                {
                    Success = false,
                    Data = null!, // TODO : fix all null! occurences
                    Error = new ApiError()
                    {
                        Code = ApiErrorCodes.NotFound,
                        Message = "refresh token doesn't exist",
                    }
                });
            }

            if(refreshTokenModel.ExpiresAt < DateTime.UtcNow)
            {
                return BadRequest(new ApiResponse()
                {
                    Success = false,
                    Data = null!,
                    Error = new ApiError()
                    {
                        Code = ApiErrorCodes.TokenExpired,
                        Message = "refresh token expired",
                    }
                });
            }

            if (refreshTokenModel.Blocked)
            {
                return Unauthorized(new ApiResponse()
                {
                    Success = false,
                    Data = null!,
                    Error = new ApiError()
                    {
                        Code = ApiErrorCodes.Unauthorized,
                        Message = "refresh token is blocked",
                    }
                });
            }

            var user = await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == refreshTokenModel.UserId);
            if (user == null)
            {
                throw new UserDoesntExistException("user doesn't exist");
            }

            var newAccessToken = await jwtHelper.GenerateAccessTokenAsync(user);

            return Ok(new ApiResponse() 
            {
                Success = true,
                Data = new { newAccessToken },
            });

        }

        [Authorize]
        [HttpGet("my-profile")]
        public async Task<ActionResult<ApiResponse>> MyProfile()
        {
            // create authentication middleware to generate User object that contains the user claims from the jwt
            // get userid from the User object
            // return the user data

            Console.WriteLine("User identity : " + User.Identity?.Name);
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var user = await _context.Users.AsNoTracking().Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == userId);
            return Ok(new ApiResponse()
            {
                Success = true,
                Data = new UserWithRoleNameDTO(user),
            });
        }

        [HttpPost("forget-password")]
        public async Task<ActionResult<ApiResponse>> ForgetPassword()
        {
            // ask for email
            // check if email exists
            // generate a token and send it within the response
            throw new NotImplementedException();
        }

        [HttpPost("reset-password")]
        public async Task<ActionResult<ApiResponse>> ResetPassword()
        {
            // get the token from the user
            // check token validity
            // get the new password
            // update the password
            throw new NotImplementedException();
        }

        // create custom code to manipulate permissions
        /*
         * for example to check if a user with role x can access an endpoint we need to check x[permissions.targetPermission] == true or not
         * 
         * DONE
         * 
         */
    }
}
