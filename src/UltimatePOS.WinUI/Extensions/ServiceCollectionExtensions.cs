using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using UltimatePOS.Core.Interfaces;
using UltimatePOS.Data;
using UltimatePOS.Data.Repositories;
using UltimatePOS.Services;
using UltimatePOS.WinUI.Services;
using System;
using System.IO;
using Microsoft.UI.Xaml.Controls;

namespace UltimatePOS.WinUI.Extensions;

/// <summary>
/// Extension methods for configuring dependency injection
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Register database context and configure SQLite
    /// </summary>
    public static IServiceCollection AddDatabase(this IServiceCollection services, string databasePath)
    {
        // Ensure database directory exists
        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var dbDirectory = Path.Combine(appDataPath, "UltimatePOS");
        Directory.CreateDirectory(dbDirectory);

        var fullDbPath = Path.Combine(dbDirectory, databasePath);

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlite($"Data Source={fullDbPath}");
            options.EnableSensitiveDataLogging(false);
            options.EnableDetailedErrors(true);
        });

        return services;
    }

    /// <summary>
    /// Register repository pattern (Unit of Work)
    /// </summary>
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        return services;
    }

    /// <summary>
    /// Register business services
    /// </summary>
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        // Infrastructure services
        services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();
        services.AddSingleton<ITokenService, TokenService>();
        services.AddSingleton<ISecureStorageService, SecureStorageService>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();

        services.AddSingleton<INavigationService, NavigationService>();
        services.AddSingleton<IMessageBus, MessageBus>();
        
        // Dialog service requires a factory for ContentDialog (needs XamlRoot)
        services.AddSingleton<IDialogService>(sp => 
            new DialogService(() => new ContentDialog()));

        // Configuration service
        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var configPath = Path.Combine(appDataPath, "UltimatePOS", "user-settings.json");
        services.AddSingleton<IConfigurationService>(new ConfigurationService(configPath));

        // Business services
        services.AddSingleton<ISessionService, SessionService>();
        services.AddScoped<IBusinessService, BusinessService>();
        services.AddScoped<ILocationService, LocationService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddSingleton<IBarcodeService, BarcodeService>();
        services.AddScoped<IStockService, StockService>();

        // Session monitoring
        services.AddHostedService<SessionMonitor>();

        // Example:
        // services.AddScoped<IProductService, ProductService>();
        // services.AddScoped<ISaleService, SaleService>();

        return services;
    }

    /// <summary>
    /// Register ViewModels
    /// </summary>
    public static IServiceCollection AddViewModels(this IServiceCollection services)
    {
        // ViewModels will be registered here as we create them
        services.AddTransient<UltimatePOS.Core.ViewModels.LoginViewModel>();
        services.AddTransient<UltimatePOS.Core.ViewModels.Business.BusinessListViewModel>();
        services.AddTransient<UltimatePOS.Core.ViewModels.Business.LocationListViewModel>();
        services.AddTransient<UltimatePOS.Core.ViewModels.DashboardViewModel>();
        services.AddTransient<UltimatePOS.Core.ViewModels.Product.ProductListViewModel>();
        services.AddTransient<UltimatePOS.Core.ViewModels.Product.ProductFormViewModel>();
        services.AddTransient<UltimatePOS.Core.ViewModels.Product.BarcodePrintViewModel>();
        services.AddTransient<UltimatePOS.Core.ViewModels.Stock.StockListViewModel>();
        services.AddTransient<UltimatePOS.Core.ViewModels.Stock.StockAdjustmentViewModel>();
        services.AddTransient<UltimatePOS.Core.ViewModels.Stock.StockTransferViewModel>();
        
        return services;
    }
}
