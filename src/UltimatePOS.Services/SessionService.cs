using System;
using CommunityToolkit.Mvvm.ComponentModel;
using UltimatePOS.Core.Entities;
using UltimatePOS.Core.Interfaces;

namespace UltimatePOS.Services;

public partial class SessionService : ObservableObject, ISessionService
{
    [ObservableProperty]
    private Business? _currentBusiness;

    [ObservableProperty]
    private Location? _currentLocation;

    [ObservableProperty]
    private User? _currentUser;

    [ObservableProperty]
    private DateTime? _sessionExpiresAt;

    public event EventHandler<Business>? BusinessChanged;
    public event EventHandler<Location>? LocationChanged;
    
    #pragma warning disable CS0067
    public event EventHandler? SessionExpired;
    #pragma warning restore CS0067

    public void SetCurrentBusiness(Business business)
    {
        CurrentBusiness = business;
        BusinessChanged?.Invoke(this, business);
    }

    public void SetCurrentLocation(Location location)
    {
        CurrentLocation = location;
        LocationChanged?.Invoke(this, location);
    }

    public void SetCurrentUser(User user)
    {
        CurrentUser = user;
    }

    public void SetSessionExpiration(DateTime? expiresAt)
    {
        SessionExpiresAt = expiresAt;
    }
}
