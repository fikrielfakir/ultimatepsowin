using System;
using System.Threading.Tasks;
using UltimatePOS.Core.Entities;
using UltimatePOS.Core.Interfaces;

namespace UltimatePOS.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly ISecureStorageService _secureStorageService;
    private const string TokenStorageKey = "session_token";
    
    public User? CurrentUser { get; private set; }
    public bool IsLoggedIn => CurrentUser != null;

    public event EventHandler<User>? UserLoggedIn;
    public event EventHandler? UserLoggedOut;
    public event EventHandler? SessionExpired;

    public AuthenticationService(
        IUnitOfWork unitOfWork, 
        IPasswordHasher passwordHasher,
        ITokenService tokenService,
        ISecureStorageService secureStorageService)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _secureStorageService = secureStorageService;
    }

    public async Task<bool> LoginAsync(string username, string password)
    {
        try
        {
            // Find user by username
            var users = await _unitOfWork.Users.FindAsync(u => u.Username == username && u.IsActive);
            var user = System.Linq.Enumerable.FirstOrDefault(users);

            if (user == null)
            {
                return false;
            }

            // Verify password
            if (_passwordHasher.VerifyPassword(password, user.PasswordHash))
            {
                // Generate token
                var token = _tokenService.GenerateToken(user);
                
                // Store token securely
                await _secureStorageService.SaveAsync(TokenStorageKey, token);
                
                CurrentUser = user;
                UserLoggedIn?.Invoke(this, user);
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            // Log exception here
            System.Diagnostics.Debug.WriteLine($"Login failed: {ex.Message}");
            return false;
        }
    }

    public async Task LogoutAsync()
    {
        try
        {
            // Get current token and revoke it
            var token = await _secureStorageService.GetAsync(TokenStorageKey);
            if (token != null)
            {
                _tokenService.RevokeToken(token);
                await _secureStorageService.DeleteAsync(TokenStorageKey);
            }

            CurrentUser = null;
            UserLoggedOut?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Logout failed: {ex.Message}");
        }
    }

    public async Task<bool> ValidateSessionAsync()
    {
        try
        {
            var token = await _secureStorageService.GetAsync(TokenStorageKey);
            if (string.IsNullOrEmpty(token))
                return false;

            var sessionToken = _tokenService.ValidateToken(token);
            if (sessionToken == null)
            {
                // Token invalid or expired
                await LogoutAsync();
                SessionExpired?.Invoke(this, EventArgs.Empty);
                return false;
            }

            // Load user if not already loaded
            if (CurrentUser == null || CurrentUser.Id != sessionToken.UserId)
            {
                CurrentUser = await _unitOfWork.Users.GetByIdAsync(sessionToken.UserId);
                if (CurrentUser != null)
                {
                    UserLoggedIn?.Invoke(this, CurrentUser);
                }
            }

            return CurrentUser != null;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Session validation failed: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> RefreshSessionAsync()
    {
        try
        {
            var token = await _secureStorageService.GetAsync(TokenStorageKey);
            if (string.IsNullOrEmpty(token))
                return false;

            var newToken = _tokenService.RefreshToken(token);
            if (newToken == null)
                return false;

            await _secureStorageService.SaveAsync(TokenStorageKey, newToken);
            return true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Session refresh failed: {ex.Message}");
            return false;
        }
    }

    public async Task<TimeSpan> GetSessionRemainingTimeAsync()
    {
        try
        {
            var token = await _secureStorageService.GetAsync(TokenStorageKey);
            if (string.IsNullOrEmpty(token))
                return TimeSpan.Zero;

            var sessionToken = _tokenService.ValidateToken(token);
            if (sessionToken == null)
                return TimeSpan.Zero;

            return sessionToken.TimeUntilExpiration;
        }
        catch
        {
            return TimeSpan.Zero;
        }
    }
}
