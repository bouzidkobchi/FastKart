using System.ComponentModel.DataAnnotations;

namespace FastKart.Models
{
    public class RefreshToken
    {
        [Key]
        public required string RefreshTokenHash { get; set; }
        public Guid UserId { get; set; }
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        public DateTime ExpiresAt { get; set; }

        public bool Blocked { get; set
            {
                if (value == true)
                {
                    field = true;
                    BlockedAt = DateTime.UtcNow;
                }
            } } = false;
        public DateTime? BlockedAt { get; private set; } = null;
    }
}
