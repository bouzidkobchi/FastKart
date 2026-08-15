using FastKart.Models;
using Microsoft.AspNetCore.Authorization;

namespace FastKart.Auth
{
    public class PermissionRequirement : IAuthorizationRequirement
    {
        public Permission Permission { get; set; }
        public PermissionRequirement(Permission permission)
        {
            Permission = permission;
        }
    }
}
