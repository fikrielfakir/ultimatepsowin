using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using UltimatePOS.Core.Interfaces;

namespace UltimatePOS.Services;

/// <summary>
/// Configuration service for managing app settings
/// </summary>
public class ConfigurationService : IConfigurationService
{
    private readonly string _configFilePath;
    private Dictionary<string, object> _settings = new();
    private readonly object _lock = new();

    public event EventHandler<ConfigurationChangedEventArgs>? ConfigurationChanged;

    public ConfigurationService(string configFilePath)
    {
        _configFilePath = configFilePath;
        LoadSettings();
    }

    public T? GetValue<T>(string key, T? defaultValue = default)
    {
        lock (_lock)
        {
            if (_settings.TryGetValue(key, out var value))
            {
                try
                {
                    if (value is JsonElement jsonElement)
                    {
                        return JsonSerializer.Deserialize<T>(jsonElement.GetRawText());
                    }
                    return (T)Convert.ChangeType(value, typeof(T));
                }
                catch
                {
                    return defaultValue;
                }
            }
            return defaultValue;
        }
    }

    public void SetValue<T>(string key, T value)
    {
        lock (_lock)
        {
            var oldValue = _settings.ContainsKey(key) ? _settings[key] : null;
            _settings[key] = value!;

            ConfigurationChanged?.Invoke(this, new ConfigurationChangedEventArgs
            {
                Key = key,
                OldValue = oldValue,
                NewValue = value
            });
        }
    }

    public async Task SaveAsync()
    {
        Dictionary<string, object> settingsCopy;
        lock (_lock)
        {
            settingsCopy = new Dictionary<string, object>(_settings);
        }

        var json = JsonSerializer.Serialize(settingsCopy, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        await File.WriteAllTextAsync(_configFilePath, json);
    }

    public async Task ReloadAsync()
    {
        LoadSettings();
        await Task.CompletedTask;
    }

    private void LoadSettings()
    {
        lock (_lock)
        {
            if (File.Exists(_configFilePath))
            {
                try
                {
                    var json = File.ReadAllText(_configFilePath);
                    _settings = JsonSerializer.Deserialize<Dictionary<string, object>>(json) 
                        ?? new Dictionary<string, object>();
                }
                catch
                {
                    _settings = new Dictionary<string, object>();
                }
            }
            else
            {
                _settings = new Dictionary<string, object>();
            }
        }
    }
}
