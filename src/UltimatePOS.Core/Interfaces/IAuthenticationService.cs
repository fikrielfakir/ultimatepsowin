using System;
using System.Threading.Tasks;
using UltimatePOS.Core.Entities;

namespace UltimatePOS.Core.Interfaces;

public interface IAuthenticationService
{
    User? CurrentUser { get; }
    bool IsLoggedIn { get; }
    
    Task<bool> LoginAsync(string username, string password);
    Task LogoutAsync();
    
    /// <summary>
    /// Validate the current session (check token validity)
    /// </summary>
    Task<bool> ValidateSessionAsync();
    
    /// <summary>
    /// Refresh the current session token
    /// </summary>
    Task<bool> RefreshSessionAsync();
    
    /// <summary>
    /// Get remaining time until session expires
    /// </summary>
    Task<TimeSpan> GetSessionRemainingTimeAsync();
    
    event EventHandler<User>? UserLoggedIn;
    event EventHandler? UserLoggedOut;
    event EventHandler? SessionExpired;
}
