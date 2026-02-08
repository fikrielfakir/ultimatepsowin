using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using UltimatePOS.Core.Interfaces;

namespace UltimatePOS.Core.ViewModels.Products;

public partial class BarcodePrintViewModel : ObservableObject
{
    private readonly IProductService _productService;
    private readonly IBarcodeService _barcodeService;
    private readonly IDialogService _dialogService;

    [ObservableProperty]
    private ObservableCollection<LabelTemplate> _templates = new();

    [ObservableProperty]
    private LabelTemplate? _selectedTemplate;

    [ObservableProperty]
    private byte[]? _previewImage;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private int _productCount;

    private int[] _productIds = Array.Empty<int>();

    public BarcodePrintViewModel(
        IProductService productService,
        IBarcodeService barcodeService,
        IDialogService dialogService)
    {
        _productService = productService;
        _barcodeService = barcodeService;
        _dialogService = dialogService;

        // Load templates
        Templates.Add(LabelTemplate.A4_40Up);
        Templates.Add(LabelTemplate.A4_24Up);
        SelectedTemplate = Templates.First();
    }

    [RelayCommand]
    public async Task LoadDataAsync(int[] productIds)
    {
        IsLoading = true;
        _productIds = productIds;
        ProductCount = productIds.Length;

        try
        {
            await GeneratePreviewAsync();
        }
        catch (Exception ex)
        {
            await _dialogService.ShowMessageAsync("Error", $"Failed to generate preview: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    public async Task GeneratePreviewAsync()
    {
        if (_productIds.Length == 0 || SelectedTemplate == null) return;

        IsLoading = true;
        try
        {
            // Fetch product details
            var labels = new System.Collections.Generic.List<BarcodeLabelData>();
            foreach (var id in _productIds)
            {
                var product = await _productService.GetProductByIdAsync(id);
                if (product != null)
                {
                    labels.Add(new BarcodeLabelData
                    {
                        Name = product.Name,
                        Price = product.SellingPrice,
                        SKU = product.SKU ?? "",
                        Barcode = product.Barcode ?? product.SKU ?? product.Id.ToString()
                    });
                }
            }

            // Generate label sheet
            PreviewImage = await _barcodeService.GenerateLabelSheetAsync(labels, SelectedTemplate);
        }
        finally
        {
            IsLoading = false;
        }
    }

    partial void OnSelectedTemplateChanged(LabelTemplate? value)
    {
        if (value != null && _productIds.Length > 0)
        {
            _ = GeneratePreviewAsync();
        }
    }

    [RelayCommand]
    public async Task PrintAsync()
    {
        if (PreviewImage == null) return;

        try
        {
            // TODO: Implement actual printing
            // For now, we'll save to a file or show success
            // In a real app, uses PrintManager or PrintHelper
            
            await _dialogService.ShowMessageAsync("Print", "Printing sent to printer successfully (simulated)");
            
            // Or allow saving as PDF/Image
            // var picker = new Windows.Storage.Pickers.FileSavePicker(); ...
        }
        catch (Exception ex)
        {
            await _dialogService.ShowMessageAsync("Error", $"Printing failed: {ex.Message}");
        }
    }
}
