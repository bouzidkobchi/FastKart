using FastKart.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace FastKart.Auth
{
    public class PermissionPolicyProvider
    : DefaultAuthorizationPolicyProvider
    {
        public PermissionPolicyProvider(
            IOptions<AuthorizationOptions> options)
            : base(options)
        {
        }

        public override async Task<AuthorizationPolicy?>
            GetPolicyAsync(string policyName)
        {

            if (!policyName.StartsWith("Permission:"))
                return await base.GetPolicyAsync(policyName);

            var permissionName =
                policyName["Permission:".Length..];

            if (!Enum.TryParse<Permission>(
                permissionName,
                out var permission))
            {
                return null;
            }

            return new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .AddRequirements(
                    new PermissionRequirement(permission))
                .Build();
        }
    }
}
