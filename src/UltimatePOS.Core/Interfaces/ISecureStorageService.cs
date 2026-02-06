using System.Threading.Tasks;

namespace UltimatePOS.Core.Interfaces;

/// <summary>
/// Service for securely storing sensitive data using platform-specific encryption
/// </summary>
public interface ISecureStorageService
{
    /// <summary>
    /// Save a value securely
    /// </summary>
    Task SaveAsync(string key, string value);

    /// <summary>
    /// Retrieve a securely stored value
    /// </summary>
    Task<string?> GetAsync(string key);

    /// <summary>
    /// Delete a securely stored value
    /// </summary>
    Task DeleteAsync(string key);

    /// <summary>
    /// Check if a key exists
    /// </summary>
    Task<bool> ContainsKeyAsync(string key);
}
