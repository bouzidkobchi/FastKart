using System.ComponentModel.DataAnnotations;

namespace FastKart.Models
{
    public class Role
    {
        [Key]
        public required string Name { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool[] Permissions { get; set; } = new bool[90];
    }

    public class RoleWithoutPermissionsDTO
    {
        public string Name { get; }
        public DateTime CreatedAt { get; }
        public int PermissionsCount { get; }
        public RoleWithoutPermissionsDTO(Role role)
        {
            Name = role.Name;
            CreatedAt = role.CreatedAt;
            PermissionsCount = role.Permissions.Count(b => b);
        }
    }
}
