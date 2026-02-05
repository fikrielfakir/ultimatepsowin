using System;
using System.Threading.Tasks;
using UltimatePOS.Core.Entities;
using UltimatePOS.Core.Interfaces;

namespace UltimatePOS.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    
    public User? CurrentUser { get; private set; }
    public bool IsLoggedIn => CurrentUser != null;

    public event EventHandler<User>? UserLoggedIn;
    public event EventHandler? UserLoggedOut;

    public AuthenticationService(IUnitOfWork unitOfWork, IPasswordHasher passwordHasher)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
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

    public Task LogoutAsync()
    {
        CurrentUser = null;
        UserLoggedOut?.Invoke(this, EventArgs.Empty);
        return Task.CompletedTask;
    }
}
