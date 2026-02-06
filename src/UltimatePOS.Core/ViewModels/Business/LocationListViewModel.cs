using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using UltimatePOS.Core.Entities;
using UltimatePOS.Core.Interfaces;

namespace UltimatePOS.Core.ViewModels.Business;

public partial class LocationListViewModel : ObservableObject
{
    private readonly ILocationService _locationService;
    private readonly ISessionService _sessionService;
    private readonly IDialogService _dialogService;

    [ObservableProperty]
    private ObservableCollection<Location> _locations = new();

    [ObservableProperty]
    private Location? _selectedLocation;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private bool _canAddLocation;

    public LocationListViewModel(
        ILocationService locationService,
        ISessionService sessionService,
        IDialogService dialogService)
    {
        _locationService = locationService;
        _sessionService = sessionService;
        _dialogService = dialogService;

        // Listen for Business changes
        _sessionService.BusinessChanged += OnBusinessChanged;
        
        // Initial check
        UpdateState();
    }

    private void OnBusinessChanged(object? sender, Core.Entities.Business e)
    {
        UpdateState();
        if (_sessionService.CurrentBusiness != null)
        {
            _ = LoadLocationsAsync();
        }
        else
        {
            Locations.Clear();
        }
    }

    private void UpdateState()
    {
        CanAddLocation = _sessionService.CurrentBusiness != null;
    }

    [RelayCommand]
    public async Task LoadLocationsAsync()
    {
        if (_sessionService.CurrentBusiness == null) return;

        IsLoading = true;
        try
        {
            var list = await _locationService.GetLocationsByBusinessIdAsync(_sessionService.CurrentBusiness.Id);
            Locations.Clear();
            foreach (var item in list)
            {
                Locations.Add(item);
            }
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task AddLocationAsync()
    {
        if (_sessionService.CurrentBusiness == null)
        {
             await _dialogService.ShowMessageAsync("Error", "Please select a business first.");
             return;
        }

        // TODO: Show Add Location Dialog
        await _dialogService.ShowMessageAsync("Not Implemented", "Add Location dialog is coming next.");
    }
}
