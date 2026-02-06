using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using UltimatePOS.Core.Entities;
using UltimatePOS.Core.Interfaces;
using UltimatePOS.Core.Models;

namespace UltimatePOS.Services;

/// <summary>
/// Token service implementation using HMAC-SHA256 signing
/// </summary>
public class TokenService : ITokenService
{
    private readonly byte[] _secretKey;
    private readonly HashSet<string> _revokedTokens = new();
    private readonly int _tokenLifetimeMinutes;

    public TokenService(IConfigurationService configurationService)
    {
        // Get or generate secret key
        var keyString = configurationService.GetValue<string>("Security:TokenSecretKey");
        if (string.IsNullOrEmpty(keyString))
        {
            // Generate new key and save
            keyString = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            configurationService.SetValue("Security:TokenSecretKey", keyString);
            _ = configurationService.SaveAsync(); // Fire and forget
        }
        _secretKey = Convert.FromBase64String(keyString);

        // Get token lifetime from config (default 30 minutes)
        _tokenLifetimeMinutes = configurationService.GetValue<int?>("Security:SessionTimeoutMinutes") ?? 30;
    }

    public string GenerateToken(User user)
    {
        var token = new SessionToken
        {
            TokenId = Guid.NewGuid().ToString(),
            UserId = user.Id,
            Username = user.Username,
            IssuedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddMinutes(_tokenLifetimeMinutes),
            LastActivityAt = DateTime.UtcNow
        };

        var payload = JsonSerializer.Serialize(token);
        var signature = ComputeSignature(payload);

        // Format: base64(payload).base64(signature)
        var tokenString = $"{Convert.ToBase64String(Encoding.UTF8.GetBytes(payload))}.{Convert.ToBase64String(signature)}";
        return tokenString;
    }

    public SessionToken? ValidateToken(string token)
    {
        try
        {
            if (string.IsNullOrEmpty(token) || IsTokenRevoked(token))
                return null;

            var parts = token.Split('.');
            if (parts.Length != 2)
                return null;

            var payload = Encoding.UTF8.GetString(Convert.FromBase64String(parts[0]));
            var signature = Convert.FromBase64String(parts[1]);

            // Verify signature
            var expectedSignature = ComputeSignature(payload);
            if (!CryptographicOperations.FixedTimeEquals(signature, expectedSignature))
                return null;

            // Deserialize and check expiration
            var sessionToken = JsonSerializer.Deserialize<SessionToken>(payload);
            if (sessionToken == null || sessionToken.IsExpired)
                return null;

            return sessionToken;
        }
        catch
        {
            return null;
        }
    }

    public string? RefreshToken(string token)
    {
        var sessionToken = ValidateToken(token);
        if (sessionToken == null)
            return null;

        // Create new token with extended expiration
        var newToken = new SessionToken
        {
            TokenId = Guid.NewGuid().ToString(),
            UserId = sessionToken.UserId,
            Username = sessionToken.Username,
            IssuedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddMinutes(_tokenLifetimeMinutes),
            LastActivityAt = DateTime.UtcNow
        };

        var payload = JsonSerializer.Serialize(newToken);
        var signature = ComputeSignature(payload);

        // Revoke old token
        RevokeToken(token);

        return $"{Convert.ToBase64String(Encoding.UTF8.GetBytes(payload))}.{Convert.ToBase64String(signature)}";
    }

    public void RevokeToken(string token)
    {
        if (!string.IsNullOrEmpty(token))
        {
            _revokedTokens.Add(token);
        }
    }

    public bool IsTokenRevoked(string token)
    {
        return _revokedTokens.Contains(token);
    }

    private byte[] ComputeSignature(string payload)
    {
        using var hmac = new HMACSHA256(_secretKey);
        return hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
    }
}
