using AssetManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

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
        public DbSet<AuditLog> AuditLogs { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Настройка уникальных полей
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

            // Инициализация начальных данных
            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "Администратор", CanManageAssets = true, CanManageUsers = true, CanViewReports = true, CanExportData = true },
                new Role { Id = 2, Name = "Редактор", CanManageAssets = true, CanViewReports = true, CanExportData = true },
                new Role { Id = 3, Name = "Просмотр", CanViewReports = true }
            );

            var adminPassword = Environment.GetEnvironmentVariable("ADMIN_INITIAL_PASSWORD") ?? "temp_password_123";
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Username = "admin",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(adminPassword),
                    FullName = "Системный администратор",
                    Email = "admin@example.com",
                    IsActive = true,
                    CreatedAt = DateTime.Now,
                    RoleId = 1
                }
            );

            modelBuilder.Entity<AssetType>().HasData(
                new AssetType { Id = 1, Name = "Компьютер" },
                new AssetType { Id = 2, Name = "Принтер" },
                new AssetType { Id = 3, Name = "Сервер" }
            );

            modelBuilder.Entity<Department>().HasData(
                new Department { Id = 1, Name = "IT-отдел" },
                new Department { Id = 2, Name = "Бухгалтерия" }
            );
        }

        public void SeedInitialData()
        {
            if (!Roles.Any())
            {
                Roles.AddRange(
                    new Role { Name = "Администратор", CanManageAssets = true, CanManageUsers = true, CanViewReports = true, CanExportData = true },
                    new Role { Name = "Редактор", CanManageAssets = true, CanViewReports = true, CanExportData = true },
                    new Role { Name = "Просмотр", CanViewReports = true }
                );
                SaveChanges();
            }

            if (!Users.Any(u => u.Username == "admin"))
            {
                var adminPassword = Environment.GetEnvironmentVariable("ADMIN_INITIAL_PASSWORD") ?? "temp_password_123";
                Users.Add(new User
                {
                    Username = "admin",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(adminPassword),
                    FullName = "Системный администратор",
                    Email = "admin@example.com",
                    IsActive = true,
                    CreatedAt = DateTime.Now,
                    RoleId = 1
                });
                SaveChanges();
            }

            if (!AssetTypes.Any())
            {
                AssetTypes.AddRange(
                    new AssetType { Name = "Компьютер" },
                    new AssetType { Name = "Принтер" },
                    new AssetType { Name = "Сервер" }
                );
                SaveChanges();
            }

            if (!Departments.Any())
            {
                Departments.AddRange(
                    new Department { Name = "IT-отдел" },
                    new Department { Name = "Бухгалтерия" }
                );
                SaveChanges();
            }
        }
    }
}