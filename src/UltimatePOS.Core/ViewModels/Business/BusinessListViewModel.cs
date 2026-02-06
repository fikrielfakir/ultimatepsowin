using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using UltimatePOS.Core.Entities;
using UltimatePOS.Core.Interfaces;

namespace UltimatePOS.Core.ViewModels.Business;

public partial class BusinessListViewModel : ObservableObject
{
    private readonly IBusinessService _businessService;
    private readonly ISessionService _sessionService;
    private readonly IDialogService _dialogService;
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    private ObservableCollection<Core.Entities.Business> _businesses = new();

    [ObservableProperty]
    private Core.Entities.Business? _selectedBusiness;

    [ObservableProperty]
    private bool _isLoading;

    public BusinessListViewModel(
        IBusinessService businessService,
        ISessionService sessionService,
        IDialogService dialogService,
        INavigationService navigationService)
    {
        _businessService = businessService;
        _sessionService = sessionService;
        _dialogService = dialogService;
        _navigationService = navigationService;
    }

    [RelayCommand]
    public async Task LoadBusinessesAsync()
    {
        IsLoading = true;
        try
        {
            var list = await _businessService.GetAllBusinessesAsync();
            Businesses.Clear();
            foreach (var item in list)
            {
                Businesses.Add(item);
            }
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task AddBusinessAsync()
    {
        // TODO: Show Add Business Dialog
        await _dialogService.ShowMessageAsync("Not Implemented", "Add Business dialog is coming next.");
    }

    [RelayCommand]
    private void SetActiveBusiness(Core.Entities.Business? business)
    {
        if (business == null) return;
        _sessionService.SetCurrentBusiness(business);
    }
}
