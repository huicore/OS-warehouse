using AssetManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace AssetManagementSystem.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Asset> Assets { get; set; }
        public DbSet<AssetType> AssetTypes { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<AssetHistory> AssetHistories { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Asset>()
                .HasIndex(a => a.InventoryNumber)
                .IsUnique();

            modelBuilder.Entity<AssetType>()
                .HasIndex(at => at.Name)
                .IsUnique();

            modelBuilder.Entity<Department>()
                .HasIndex(d => d.Name)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            var adminRole = new Role
            {
                Id = 1,
                Name = "Администратор",
                CanViewAssets = true,
                CanEditAssets = true,
                CanDeleteAssets = true,
                CanAddAssets = true,
                CanManageUsers = true,
                CanGenerateReports = true
            };

            modelBuilder.Entity<Role>().HasData(adminRole);

            var adminUser = new User
            {
                Id = 1,
                Username = "admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                FullName = "Администратор системы",
                Email = "admin@company.com",
                IsActive = true,
                CreatedAt = DateTime.Now,
                RoleId = 1
            };

            modelBuilder.Entity<User>().HasData(adminUser);
        }
    }
}