using FastKart.Helpers;
using FluentValidation;

namespace FastKart.Models
{
    public class AppUser
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public required string Name { get; set; }
        public required string Email { get; set; }
        public Role Role { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool Status { get; set; } = true; // false if blocked, true otherwise
        public required string PasswordHash { get; set; }
        public required string Phone { get; set; }

        public void UpdatePassword(string password)
        {
            PasswordHash = PasswordHasher.CreatePasswordHash(password);
        }
    }
}
