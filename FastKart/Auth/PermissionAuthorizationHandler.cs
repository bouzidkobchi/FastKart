using FastKart.Models.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FastKart.Auth
{
    public class PermissionAuthorizationHandler
        : AuthorizationHandler<PermissionRequirement>
    {
        private readonly AppDbContext _context;

        public PermissionAuthorizationHandler(AppDbContext context)
        {
            _context = context;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            PermissionRequirement requirement)
        {
            if (!Guid.TryParse(
                context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                out var userId))
            {
                return;
            }

            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user?.Role == null)
                return;

            var permissions = user.Role.Permissions;

            var index = (int)requirement.Permission;

            if (index < permissions.Length && permissions[index])
            {
                context.Succeed(requirement);
            }
        }
    }
}