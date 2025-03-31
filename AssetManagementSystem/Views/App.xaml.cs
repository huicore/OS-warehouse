using AssetManagementSystem.Data;
using AssetManagementSystem.Services;
using AssetManagementSystem.ViewModels;
using Microsoft.Extensions.DependencyInjection;

private IServiceProvider ConfigureServices()
{
    var services = new ServiceCollection();

    // Database
    services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString));

    // Services
    services.AddSingleton<ExportService>();
    services.AddSingleton<AssetService>();
    services.AddSingleton<AuthorizationService>();
    services.AddSingleton<AuditService>();

    // ViewModels
    services.AddTransient<LoginViewModel>();
    services.AddSingleton<MainWindowViewModel>();
    services.AddTransient<AssetListViewModel>();
    services.AddTransient<UserManagementViewModel>();
    services.AddTransient<AuditLogViewModel>();

    return services.BuildServiceProvider();
}