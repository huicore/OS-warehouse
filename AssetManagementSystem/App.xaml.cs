using AssetManagementSystem.Data;
using AssetManagementSystem.Services;
using AssetManagementSystem.ViewModels;
using AssetManagementSystem.Views;
using AssetManagementSystem.Views.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Configuration;
using System.Windows;

namespace AssetManagementSystem
{
    public partial class App : Application
    {
        private IServiceProvider _serviceProvider;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Конфигурация сервисов
            _serviceProvider = ConfigureServices();

            // Проверка и применение миграций БД
            ApplyMigrations();

            // Инициализация главного окна
            var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

        private IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            // Конфигурация базы данных
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlite("Data Source=AssetManagementSystem.db");
                options.EnableDetailedErrors();
                options.EnableSensitiveDataLogging();
            }, ServiceLifetime.Transient);

            // Регистрация сервисов
            services.AddSingleton<ExportService>();
            services.AddSingleton<AuditService>();
            services.AddSingleton<CurrentUser>();

            // Регистрация ViewModels
            services.AddTransient<LoginViewModel>();
            services.AddTransient<AssetListViewModel>();
            services.AddTransient<AssetEditViewModel>();
            services.AddTransient<UserListViewModel>();
            services.AddTransient<UserEditViewModel>();
            services.AddTransient<ReportViewModel>();
            services.AddTransient<DashboardViewModel>();

            // Регистрация окон
            services.AddSingleton<MainWindow>();
            services.AddTransient<LoginWindow>();

            return services.BuildServiceProvider();
        }

        private void ApplyMigrations()
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                // Применяем миграции, если они есть
                dbContext.Database.Migrate();

                // Проверяем, есть ли администратор в системе
                if (!dbContext.Users.Any(u => u.Username == "admin"))
                {
                    // Создаем начальные данные
                    dbContext.SeedInitialData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка инициализации базы данных: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Current.Shutdown();
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            // Очистка ресурсов
            if (_serviceProvider is IDisposable disposable)
            {
                disposable.Dispose();
            }

            base.OnExit(e);
        }
    }
}