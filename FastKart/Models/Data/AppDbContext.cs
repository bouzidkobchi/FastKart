using FastKart.Models.Configurations;
using Microsoft.EntityFrameworkCore;

namespace FastKart.Models.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
           
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var permissionsArray = Enumerable.Repeat(true, 90).ToArray();
            modelBuilder.Entity<Role>()
            .HasData(new Role
            {
                Name = "Client",
                Permissions = permissionsArray,
                CreatedAt = new DateTime(2026, 8, 13, 0, 0, 0, DateTimeKind.Utc)
            });

            modelBuilder.ApplyConfiguration(new AppUserConfiguration());
        }

        public DbSet<AppUser> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
    }
}
