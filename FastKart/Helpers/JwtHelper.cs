using FastKart.Models;
using FastKart.Models.Data;
using FastKart.Models.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace FastKart.Helpers
{
    public class JwtHelper(JwtOptions JWToptions)
    {
        public async Task<string> GenerateAccessTokenAsync(AppUser user)
        {
            var handler = new JwtSecurityTokenHandler();
            var securityToken = handler.CreateToken(new SecurityTokenDescriptor()
            {
                Audience = JWToptions.Audience,
                Issuer = JWToptions.Issuer,
                Expires = DateTime.Now.AddMinutes(JWToptions.LifeTime),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JWToptions.SigningKey)), SecurityAlgorithms.HmacSha256),

                Subject = new ClaimsIdentity([
                    new Claim(ClaimTypes.Name , user.Name),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                        new Claim(ClaimTypes.Role, user.Role.Name)
                    ], "Bearer")
            });

            var token = handler.WriteToken(securityToken);
            return token;
        }

        public string GenerateRandomRefreshToken()
        {
            byte[] bytes = RandomNumberGenerator.GetBytes(64);

            return Convert.ToBase64String(bytes)
                .Replace('+', '-')
                .Replace('/', '_')
                .TrimEnd('=');
        }

        public string Hash(string value)
        {
            byte[] hash = SHA256.HashData(Encoding.UTF8.GetBytes(value));

            return Convert.ToBase64String(hash);
        }
    }
}
