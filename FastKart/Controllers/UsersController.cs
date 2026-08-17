using FastKart.Auth;
using FastKart.Helpers;
using FastKart.Models;
using FastKart.Models.Data;
using FastKart.Models.DTOs;
using FastKart.Models.Requests;
using FastKart.Models.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using static FastKart.Controllers.AuthController;

namespace FastKart.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public partial class UsersController : ControllerBase
    {
        private readonly AppDbContext context;

        public UsersController(AppDbContext context)
        {
            this.context = context;
        }

        // add user
        [HttpPost("add-user")]
        [HasPermission(Permission.UsersCreate)]
        public async Task<ActionResult<ApiResponse>> AddUser(AddUserModel addUserModel, [FromServices] AddUSerValidator validator)
        {
            var validationResult = validator.Validate(addUserModel);
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

            var emailExists = await context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == addUserModel.Email) != null;
            if (emailExists)
            {
                return BadRequest("email already exists");
            }

            var roleName = addUserModel.Role;
            var role = await context.Roles.FindAsync(roleName) ;

            if(role != null)
            {

                var user = new AppUser()
                {
                    Name = addUserModel.Name,
                    Email = addUserModel.Email,
                    PasswordHash = PasswordHasher.CreatePasswordHash(addUserModel.Password),
                    Role = role,
                    Phone = addUserModel.Phone,
                };

                await context.AddAsync(user);
                await context.SaveChangesAsync();

                return Ok(new ApiResponse()
                {
                    Success = true,
                    Data = user,
                });
            }

            return BadRequest(new ApiResponse()
            {
                Success = false,
                Data = null!,
                Error = new ApiError()
                {
                    Code = ApiErrorCodes.RoleNotFound,
                    Message = "Role doesn't exist",
                }
            });
        }

        // get users
        [HttpGet("users")]
        [HasPermission(Permission.UsersIndex)]
        public async Task<ActionResult<ApiResponse>> GetUsers([FromQuery] ResponsePage page, [FromServices] ResponsePageValidator validator)
        {
            var validationResult = validator.Validate(page);

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

            var users = await context.Users.Include(u => u.Role)
                .Skip(page.Number * page.Count)
                .Take(page.Count)
                .Select(u => new UserWithRoleNameDTO(u))
                .ToListAsync();

            return Ok(new ApiResponse()
            {
                Success = true,
                Data = users
            });
        }

        // add role
        [HttpPost("roles")]
        [HasPermission(Permission.RolesCreate)]
        public async Task<ActionResult<ApiResponse>> AddRole(string roleName, List<string> permissions) // TODO : create custom type for the input
        {
            var newRole = new Role()
            {
                Name = roleName,
            };

            foreach (var permission in permissions)
            {
                bool parsingSucceed = Enum.TryParse(permission, out Permission result);
                if(parsingSucceed)
                {
                    newRole.Permissions[(int)result] = true;
                    Console.WriteLine($"{result} added successfuly to the permissions array at index {(int)result}");
                }
                else
                {
                    Console.WriteLine($"wrong permissions {permission}");
                    return BadRequest(new ApiResponse()
                    {
                        Success = false,
                        Data = new { possibleValues = Enum.GetNames(typeof(Permission)).ToList() },
                        Error = new ApiError()
                        {
                            Code = ApiErrorCodes.OutOfSetPermission,
                            Message = "permission should be part of defined set",
                        }
                    });
                }
            }

            var roleExists = await context.Roles.FindAsync(roleName) != null;

            if (roleExists)
            {
                return BadRequest(new ApiResponse()
                {
                    Success = false,
                    Data = null!,
                    Error = new ApiError()
                    {
                        Code = ApiErrorCodes.RoleAlreadyExists,
                        Message = "a role with this name already exists",
                    }
                });
            }


            await context.AddAsync(newRole);
            await context.SaveChangesAsync();


            return Ok(new ApiResponse()
            {
                Success = true,
                Data = newRole,
            });
        }

        // get roles
        [HttpGet("roles")]
        [HasPermission(Permission.RolesIndex)]
        public async Task<ActionResult<ApiResponse>> GetRoles([FromQuery] ResponsePage page, [FromServices] ResponsePageValidator validator)
        {
            var validationResult = validator.Validate(page);

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

            var roles = await context.Roles
                .AsNoTracking()
                .Skip(page.Number * page.Count)
                .Take(page.Count)
                .Select(r => new RoleWithoutPermissionsDTO(r))
                .ToListAsync();

            return Ok(new ApiResponse()
            {
                Success = true,
                Data = roles
            });
        }

        // update my info
        [HttpPatch("my-info")]
        [Authorize]
        public async Task<ActionResult<ApiResponse>> UpdateMyInfo(UpdateMyInfoModel model, [FromServices] UpdateMyInfoModelValidator validator)
        {
            var validationResult = validator.Validate(model);

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

            var userId = User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;

            var user = await context.Users.FindAsync(Guid.Parse(userId)) ?? throw new UserDoesntExistException();

            if (model.Name != null)
            {
                user.Name = model.Name;
            }

            if (model.Email != null)
            {
                user.Email = model.Email;
            }

            if(model.Phone != null)
            {
                user.Phone = model.Phone;
            }

            if(model.Password != null)
            {
                user.UpdatePassword(model.Password);
            }

            await context.SaveChangesAsync();

            return Ok(new ApiResponse()
            {
                Success = true,
                Data = "info updated successfuly!"
            });
        }

        // update another user role
        [HttpPatch("update-role")]
        [HasPermission(Permission.UsersEdit)]
        public async Task<ActionResult<ApiResponse>> UpdateUserRole(UpdateUserRoleModel model, [FromServices] UpdateUserRoleModelValidator validator)
        {
            var validationResult = validator.Validate(model);

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

            var currentUserId = Guid.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            if(currentUserId == model.UserId)
            {
                return BadRequest(new ApiResponse() { Success = false, Data = null , Error = new ApiError() { Code = ApiErrorCodes.InsufficientPermissions , Message = "user can not update his role"} });
            }

            var role = await context.Roles.FindAsync(model.Role);
            if (role  == null)
            {
                return BadRequest(new ApiResponse() { Success = false, Data = null, Error = new ApiError() { Code = ApiErrorCodes.RoleNotFound, Message = "role not found" } });
            }

            var user = await context.Users.FindAsync(model.UserId);
            if (user  == null)
            {
                return BadRequest(new ApiResponse() { Success = false, Data = null, Error = new ApiError() { Code = ApiErrorCodes.UserNotFound, Message = "user not found" } });
            }

            user.Role = role;
            await context.SaveChangesAsync();

            // TODO : add in case the user was admin/allowed to update user role and want to change his role (downgrade)

            return Ok(new ApiResponse() { Success = true, Data = "role updated successfuly"});
        }

        // block/ublock user / update user status
        [HttpPut("update-status")]
        [HasPermission(Permission.UsersEdit)]
        public async Task<ActionResult<ApiResponse>> UpdateUserStatus(UpdateUserStatusModel model,[FromServices] UpdateUserStatusModelValidator validator)
        {
            var validationResult = validator.Validate(model);

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

            // deny user from blocking himself
            var currentUserId = Guid.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            if (currentUserId == model.UserId)
            {
                return BadRequest(new ApiResponse() { Success = false, Data = null, Error = new ApiError() { Code = ApiErrorCodes.InsufficientPermissions, Message = "user can not update his status" } });
            }

            var user = await context.Users.FindAsync(model.UserId);
            if (user == null)
            {
                return BadRequest(new ApiResponse() { Success = false, Data = null, Error = new ApiError() { Code = ApiErrorCodes.UserNotFound, Message = "user not found" } });
            }

            if(user.Status == model.IsApproved == true)
            {
                return BadRequest(new ApiResponse() { Success = false, Data = null, Error = new ApiError() { Code = ApiErrorCodes.UserNotFound, Message = "user already approved" } });
            }

            if (user.Status == model.IsApproved == false)
            {
                return BadRequest(new ApiResponse() { Success = false, Data = null, Error = new ApiError() { Code = ApiErrorCodes.UserNotFound, Message = "user already blocked" } });
            }

            
            user.Status = true;
            await context.SaveChangesAsync();
            return Ok(new ApiResponse() { Success = true, Data = "user status updated successfuly" });
        }
    }
}
