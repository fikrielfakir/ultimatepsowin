using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UltimatePOS.Core.Interfaces;

namespace UltimatePOS.WinUI.Services;

/// <summary>
/// Secure storage service using Windows Data Protection API (DPAPI)
/// </summary>
public class SecureStorageService : ISecureStorageService
{
    private readonly string _storageDirectory;

    public SecureStorageService()
    {
        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        _storageDirectory = Path.Combine(appDataPath, "UltimatePOS", "SecureStorage");
        Directory.CreateDirectory(_storageDirectory);
    }

    public Task SaveAsync(string key, string value)
    {
        try
        {
            var filePath = GetFilePath(key);
            var plainBytes = Encoding.UTF8.GetBytes(value);
            
            // Encrypt using DPAPI (CurrentUser scope)
            var encryptedBytes = ProtectedData.Protect(
                plainBytes,
                null, // No additional entropy
                DataProtectionScope.CurrentUser
            );

            File.WriteAllBytes(filePath, encryptedBytes);
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Secure storage save failed: {ex.Message}");
            throw;
        }
    }

    public Task<string?> GetAsync(string key)
    {
        try
        {
            var filePath = GetFilePath(key);
            if (!File.Exists(filePath))
                return Task.FromResult<string?>(null);

            var encryptedBytes = File.ReadAllBytes(filePath);
            
            // Decrypt using DPAPI
            var plainBytes = ProtectedData.Unprotect(
                encryptedBytes,
                null,
                DataProtectionScope.CurrentUser
            );

            var value = Encoding.UTF8.GetString(plainBytes);
            return Task.FromResult<string?>(value);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Secure storage get failed: {ex.Message}");
            return Task.FromResult<string?>(null);
        }
    }

    public Task DeleteAsync(string key)
    {
        try
        {
            var filePath = GetFilePath(key);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Secure storage delete failed: {ex.Message}");
            throw;
        }
    }

    public Task<bool> ContainsKeyAsync(string key)
    {
        var filePath = GetFilePath(key);
        return Task.FromResult(File.Exists(filePath));
    }

    private string GetFilePath(string key)
    {
        // Sanitize key for file system
        var sanitizedKey = Convert.ToBase64String(Encoding.UTF8.GetBytes(key))
            .Replace("/", "_")
            .Replace("+", "-");
        return Path.Combine(_storageDirectory, $"{sanitizedKey}.dat");
    }
}
