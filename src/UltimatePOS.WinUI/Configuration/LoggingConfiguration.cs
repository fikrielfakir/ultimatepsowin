using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using System;
using System.IO;

namespace UltimatePOS.WinUI.Configuration;

/// <summary>
/// Serilog logging configuration
/// </summary>
public static class LoggingConfiguration
{
    public static void ConfigureLogging(string logDirectory, string minimumLevel, int retentionDays, long fileSizeLimitBytes)
    {
        // Ensure log directory exists
        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var logPath = Path.Combine(appDataPath, "UltimatePOS", logDirectory);
        Directory.CreateDirectory(logPath);

        var logFilePath = Path.Combine(logPath, "ultimatepos-.log");

        // Parse minimum log level
        var logLevel = ParseLogLevel(minimumLevel);

        // Configure Serilog
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Is(logLevel)
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("System", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.Debug(
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
            .WriteTo.File(
                path: logFilePath,
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: retentionDays,
                fileSizeLimitBytes: fileSizeLimitBytes,
                rollOnFileSizeLimit: true,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
            .CreateLogger();

        Log.Information("UltimatePOS application starting...");
        Log.Information("Log directory: {LogPath}", logPath);
    }

    public static ILoggerFactory CreateLoggerFactory()
    {
        return LoggerFactory.Create(builder =>
        {
            builder.AddSerilog(dispose: true);
        });
    }

    private static LogEventLevel ParseLogLevel(string level)
    {
        return level?.ToLowerInvariant() switch
        {
            "verbose" => LogEventLevel.Verbose,
            "debug" => LogEventLevel.Debug,
            "information" => LogEventLevel.Information,
            "warning" => LogEventLevel.Warning,
            "error" => LogEventLevel.Error,
            "fatal" => LogEventLevel.Fatal,
            _ => LogEventLevel.Information
        };
    }

    public static void CloseAndFlush()
    {
        Log.Information("UltimatePOS application shutting down...");
        Log.CloseAndFlush();
    }
}
