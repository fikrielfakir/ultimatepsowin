using Microsoft.UI.Xaml;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.IO;
using System.Text.Json;
using UltimatePOS.Core.Models;
using UltimatePOS.WinUI.Configuration;
using UltimatePOS.WinUI.Extensions;

namespace UltimatePOS.WinUI;

public partial class App : Application
{
    private IHost? _host;
    private Window? _window;
    
    public static Window? CurrentWindow => (App.Current as App)?._window;

    private AppSettings? _appSettings;

    public App()
    {
        this.InitializeComponent();
        
        LoadConfiguration();
        ConfigureLogging();
        ConfigureServices();
    }

    private void LoadConfiguration()
    {
        try
        {
            var configPath = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
            if (File.Exists(configPath))
            {
                var json = File.ReadAllText(configPath);
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    AllowTrailingCommas = true,
                    ReadCommentHandling = JsonCommentHandling.Skip
                };
                _appSettings = JsonSerializer.Deserialize<AppSettings>(json, options);
            }
            _appSettings ??= new AppSettings();
        }
        catch (Exception ex)
        {
            // Fallback to defaults if config loading fails
            _appSettings = new AppSettings();
            System.Diagnostics.Debug.WriteLine($"Error loading configuration: {ex.Message}");
        }
    }

    private void ConfigureLogging()
    {
        LoggingConfiguration.ConfigureLogging(
            _appSettings?.Logging.LogDirectory ?? "Logs",
            _appSettings?.Logging.MinimumLevel ?? "Information",
            _appSettings?.Logging.RetentionDays ?? 30,
            _appSettings?.Logging.FileSizeLimitBytes ?? 10485760
        );

        Log.Information("UltimatePOS WinUI 3 application initializing...");
    }

    private void ConfigureServices()
    {
        _host = Host.CreateDefaultBuilder()
            .UseSerilog()
            .ConfigureServices((context, services) =>
            {
                // Register database
                services.AddDatabase(_appSettings?.Database.DatabasePath ?? "ultimatepos.db");

                // Register repositories
                services.AddRepositories();

                // Register services
                services.AddServices();

                // Register ViewModels
                services.AddViewModels();

                Log.Information("Dependency injection configured successfully");
            })
            .Build();
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        Log.Information("Application launched");
        
        _window = new MainWindow();
        _window.Activate();
    }

    /// <summary>
    /// Get a service from the DI container
    /// </summary>
    public static T GetService<T>() where T : class
    {
        if ((App.Current as App)?._host?.Services.GetService(typeof(T)) is not T service)
        {
            throw new ArgumentException($"{typeof(T)} needs to be registered in ConfigureServices within App.xaml.cs.");
        }

        return service;
    }

    /// <summary>
    /// Cleanup on application exit
    /// </summary>
    ~App()
    {
        LoggingConfiguration.CloseAndFlush();
        _host?.Dispose();
    }
}

