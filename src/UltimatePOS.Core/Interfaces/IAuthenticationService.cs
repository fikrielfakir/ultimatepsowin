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
    
    event EventHandler<User>? UserLoggedIn;
    event EventHandler? UserLoggedOut;
}
