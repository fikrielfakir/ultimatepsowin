using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using UltimatePOS.Core.Interfaces;

namespace UltimatePOS.WinUI.Services;

/// <summary>
/// Background service that monitors session expiration and handles automatic token refresh
/// </summary>
public class SessionMonitor : IHostedService, IDisposable
{
    private readonly IAuthenticationService _authenticationService;
    private readonly ISessionService _sessionService;
    private Timer? _timer;
    private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(1); // Check every minute
    private readonly TimeSpan _refreshThreshold = TimeSpan.FromMinutes(5); // Refresh when less than 5 min remaining

    public SessionMonitor(
        IAuthenticationService authenticationService,
        ISessionService sessionService)
    {
        _authenticationService = authenticationService;
        _sessionService = sessionService;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        // Validate session on startup (auto-login if token exists)
        _ = ValidateAndRestoreSessionAsync();

        // Start periodic check
        _timer = new Timer(DoWork, null, TimeSpan.Zero, _checkInterval);
        
        return Task.CompletedTask;
    }

    private async void DoWork(object? state)
    {
        try
        {
            if (!_authenticationService.IsLoggedIn)
            {
                // Try to restore session from stored token
                await ValidateAndRestoreSessionAsync();
                return;
            }

            // Check remaining time
            var remainingTime = await _authenticationService.GetSessionRemainingTimeAsync();
            
            // Update session service
            if (remainingTime > TimeSpan.Zero)
            {
                _sessionService.SetSessionExpiration(DateTime.UtcNow.Add(remainingTime));
            }

            // Refresh token if close to expiration
            if (remainingTime > TimeSpan.Zero && remainingTime < _refreshThreshold)
            {
                var refreshed = await _authenticationService.RefreshSessionAsync();
                if (refreshed)
                {
                    System.Diagnostics.Debug.WriteLine("Session token refreshed automatically");
                }
            }
            // Session expired
            else if (remainingTime <= TimeSpan.Zero)
            {
                System.Diagnostics.Debug.WriteLine("Session expired");
                await _authenticationService.LogoutAsync();
                _sessionService.SetSessionExpiration(null);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"SessionMonitor error: {ex.Message}");
        }
    }

    private async Task ValidateAndRestoreSessionAsync()
    {
        try
        {
            var isValid = await _authenticationService.ValidateSessionAsync();
            if (isValid)
            {
                System.Diagnostics.Debug.WriteLine("Session restored from token");
                var remainingTime = await _authenticationService.GetSessionRemainingTimeAsync();
                _sessionService.SetSessionExpiration(DateTime.UtcNow.Add(remainingTime));
                
                // Set current user in session service
                if (_authenticationService.CurrentUser != null)
                {
                    _sessionService.SetCurrentUser(_authenticationService.CurrentUser);
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Session restore failed: {ex.Message}");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}
