namespace UltimatePOS.Core.Models;

/// <summary>
/// Strongly-typed application settings
/// </summary>
public class AppSettings
{
    public DatabaseSettings Database { get; set; } = new();
    public ThemeSettings Theme { get; set; } = new();
    public LocalizationSettings Localization { get; set; } = new();
    public LoggingSettings Logging { get; set; } = new();
    public GeneralSettings General { get; set; } = new();
}

public class DatabaseSettings
{
    public string Provider { get; set; } = "SQLite";
    public string ConnectionString { get; set; } = string.Empty;
    public string DatabasePath { get; set; } = string.Empty;
}

public class ThemeSettings
{
    public string Mode { get; set; } = "System"; // Light, Dark, System
    public string AccentColor { get; set; } = "#0078D4";
}

public class LocalizationSettings
{
    public string DefaultLanguage { get; set; } = "en-US";
    public string[] AvailableLanguages { get; set; } = { "en-US", "fr-FR", "ar-AR" };
}

public class LoggingSettings
{
    public string MinimumLevel { get; set; } = "Information";
    public string LogDirectory { get; set; } = "Logs";
    public int RetentionDays { get; set; } = 30;
    public long FileSizeLimitBytes { get; set; } = 10485760; // 10 MB
}

public class GeneralSettings
{
    public string Currency { get; set; } = "USD";
    public string CurrencySymbol { get; set; } = "$";
    public string DateFormat { get; set; } = "yyyy-MM-dd";
    public string TimeFormat { get; set; } = "HH:mm:ss";
    public string TimeZone { get; set; } = "UTC";
    public int SessionTimeoutMinutes { get; set; } = 30;
}
