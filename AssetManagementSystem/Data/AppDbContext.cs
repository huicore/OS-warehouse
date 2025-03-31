using AssetManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using System;

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
        public DbSet<AuditLog> AuditLogs { get; set; } // Добавлено для аудита

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
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Роли
            var adminRole = new Role
            {
                Id = 1,
                Name = "Администратор",
                Description = "Полный доступ",
                CanManageAssets = true,
                CanManageUsers = true,
                CanViewReports = true,
                CanExportData = true
            };

            modelBuilder.Entity<Role>().HasData(adminRole);

            // Администратор (пароль из переменной окружения)
            var adminPassword = Environment.GetEnvironmentVariable("ADMIN_INITIAL_PASSWORD") ?? "temp_password_123";
            var adminUser = new User
            {
                Id = 1,
                Username = "admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(adminPassword),
                FullName = "Системный администратор",
                Email = "admin@example.com",
                IsActive = true,
                CreatedAt = DateTime.Now,
                RoleId = 1
            };

            modelBuilder.Entity<User>().HasData(adminUser);

            // Типы ОС
            var assetTypes = new[]
            {
                new AssetType { Id = 1, Name = "Компьютер" },
                new AssetType { Id = 2, Name = "Принтер" },
                new AssetType { Id = 3, Name = "Сервер" }
            };

            modelBuilder.Entity<AssetType>().HasData(assetTypes);

            // Подразделения
            var departments = new[]
            {
                new Department { Id = 1, Name = "IT-отдел" },
                new Department { Id = 2, Name = "Бухгалтерия" }
            };

            modelBuilder.Entity<Department>().HasData(departments);
        }
    }
}