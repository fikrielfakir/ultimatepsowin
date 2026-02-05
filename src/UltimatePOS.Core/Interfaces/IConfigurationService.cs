using System;
using System.Threading.Tasks;

namespace UltimatePOS.Core.Interfaces;

/// <summary>
/// Service for managing application configuration and settings
/// </summary>
public interface IConfigurationService
{
    /// <summary>
    /// Get a configuration value by key
    /// </summary>
    T? GetValue<T>(string key, T? defaultValue = default);

    /// <summary>
    /// Set a configuration value
    /// </summary>
    void SetValue<T>(string key, T value);

    /// <summary>
    /// Save configuration changes to disk
    /// </summary>
    Task SaveAsync();

    /// <summary>
    /// Reload configuration from disk
    /// </summary>
    Task ReloadAsync();

    /// <summary>
    /// Event raised when configuration changes
    /// </summary>
    event EventHandler<ConfigurationChangedEventArgs>? ConfigurationChanged;
}

/// <summary>
/// Event arguments for configuration change events
/// </summary>
public class ConfigurationChangedEventArgs : EventArgs
{
    public string Key { get; set; } = string.Empty;
    public object? OldValue { get; set; }
    public object? NewValue { get; set; }
}
