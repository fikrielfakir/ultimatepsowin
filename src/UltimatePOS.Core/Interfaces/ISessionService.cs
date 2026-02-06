using System;
using System.ComponentModel;
using UltimatePOS.Core.Entities;
using UltimatePOS.Core.Models;

namespace UltimatePOS.Core.Interfaces;

public interface ISessionService : INotifyPropertyChanged
{
    Business? CurrentBusiness { get; }
    Location? CurrentLocation { get; }
    User? CurrentUser { get; }
    DateTime? SessionExpiresAt { get; }

    void SetCurrentBusiness(Business business);
    void SetCurrentLocation(Location location);
    void SetCurrentUser(User user);
    void SetSessionExpiration(DateTime? expiresAt);
    
    event EventHandler<Business> BusinessChanged;
    event EventHandler<Location> LocationChanged;
    event EventHandler? SessionExpired;
}
