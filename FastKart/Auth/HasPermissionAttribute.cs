using FastKart.Models;
using Microsoft.AspNetCore.Authorization;

namespace FastKart.Auth
{
    public class HasPermissionAttribute : AuthorizeAttribute
    {
        public HasPermissionAttribute(Permission permission)
        {
            Policy = $"Permission:{permission}";
        }
    }
}
