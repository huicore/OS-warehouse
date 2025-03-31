using AssetManagementSystem.Data;
using AssetManagementSystem.Services;
using AssetManagementSystem.ViewModels;
using AssetManagementSystem.Views.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace AssetManagementSystem
{
    public partial class App : Application
    {
        public IServiceProvider Services { get; }

        public App()
        {
            Services = ConfigureServices();
        }

        private IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            // Database
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString));

            // Services
            services.AddSingleton<ExportService>();

            // ViewModels
            services.AddTransient<LoginViewModel>();
            services.AddSingleton<MainWindowViewModel>();
            services.AddTransient<AssetListViewModel>();
            services.AddTransient<UserManagementViewModel>();

            return services.BuildServiceProvider();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var loginWindow = new LoginWindow
            {
                DataContext = Services.GetRequiredService<LoginViewModel>()
            };
            loginWindow.Show();
        }
    }
}