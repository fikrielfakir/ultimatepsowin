using UltimatePOS.Core.Entities;
using UltimatePOS.Core.Models;

namespace UltimatePOS.Core.Interfaces;

/// <summary>
/// Service for generating, validating, and managing session tokens
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Generate a new token for the user
    /// </summary>
    string GenerateToken(User user);

    /// <summary>
    /// Validate a token string and return the session token if valid
    /// </summary>
    SessionToken? ValidateToken(string token);

    /// <summary>
    /// Refresh a token, extending its expiration
    /// </summary>
    string? RefreshToken(string token);

    /// <summary>
    /// Revoke a token (add to blacklist)
    /// </summary>
    void RevokeToken(string token);

    /// <summary>
    /// Check if a token is revoked
    /// </summary>
    bool IsTokenRevoked(string token);
}
