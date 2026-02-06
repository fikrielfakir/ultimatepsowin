using System;

namespace UltimatePOS.Core.Models;

/// <summary>
/// Represents a session token with user claims and expiration information
/// </summary>
public class SessionToken
{
    /// <summary>
    /// Unique identifier for this token
    /// </summary>
    public string TokenId { get; set; } = string.Empty;

    /// <summary>
    /// User ID associated with this session
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Username associated with this session
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// When the token was issued
    /// </summary>
    public DateTime IssuedAt { get; set; }

    /// <summary>
    /// When the token expires
    /// </summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// Last activity timestamp (for session timeout tracking)
    /// </summary>
    public DateTime? LastActivityAt { get; set; }

    /// <summary>
    /// Check if the token is expired
    /// </summary>
    public bool IsExpired => DateTime.UtcNow > ExpiresAt;

    /// <summary>
    /// Get remaining time until expiration
    /// </summary>
    public TimeSpan TimeUntilExpiration => ExpiresAt - DateTime.UtcNow;
}
