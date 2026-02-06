using System;
using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using UltimatePOS.Core.Entities;
using UltimatePOS.Core.Interfaces;

namespace UltimatePOS.Core.ViewModels.Product;

public partial class ProductFormViewModel : ObservableObject
{
    private readonly IProductService _productService;
    private readonly IDialogService _dialogService;
    private readonly ISessionService _sessionService;

    [ObservableProperty]
    private int? _productId;

    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private string? _sku;

    [ObservableProperty]
    private string? _barcode;

    [ObservableProperty]
    private ProductType _productType = ProductType.Single;

    [ObservableProperty]
    private int? _brandId;

    [ObservableProperty]
    private int? _categoryId;

    [ObservableProperty]
    private int? _subCategoryId;

    [ObservableProperty]
    private int? _unitId;

    [ObservableProperty]
    private decimal _costPrice;

    [ObservableProperty]
    private decimal _sellingPrice;

    [ObservableProperty]
    private decimal _profitMargin;

    [ObservableProperty]
    private decimal _alertQuantity;

    [ObservableProperty]
    private string? _description;

    [ObservableProperty]
    private string? _imagePath;

    [ObservableProperty]
    private bool _isActive = true;

    [ObservableProperty]
    private bool _hasExpiryDate;

    [ObservableProperty]
    private bool _isSaving;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private byte[]? _imageData;

    [ObservableProperty]
    private bool _hasImage;

    // Validation errors
    [ObservableProperty]
    private string? _nameError;

    [ObservableProperty]
    private string? _skuError;

    [ObservableProperty]
    private string? _costPriceError;

    [ObservableProperty]
    private string? _sellingPriceError;

    public bool IsEditMode => ProductId.HasValue;
    public string PageTitle => IsEditMode ? "Edit Product" : "Add Product";

    public ProductFormViewModel(
        IProductService productService,
        IDialogService dialogService,
        ISessionService sessionService)
    {
        _productService = productService;
        _dialogService = dialogService;
        _sessionService = sessionService;

        // Auto-calculate profit margin when prices change
        PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(CostPrice) || e.PropertyName == nameof(SellingPrice))
            {
                CalculateProfitMargin();
            }
        };
    }

    partial void OnCostPriceChanged(decimal value)
    {
        CalculateProfitMargin();
    }

    partial void OnSellingPriceChanged(decimal value)
    {
        CalculateProfitMargin();
    }

    private void CalculateProfitMargin()
    {
        if (CostPrice > 0)
        {
            ProfitMargin = Math.Round(((SellingPrice - CostPrice) / CostPrice) * 100, 2);
        }
        else
        {
            ProfitMargin = 0;
        }
    }

    [RelayCommand]
    public async Task LoadProductAsync(int productId)
    {
        IsLoading = true;
        try
        {
            var product = await _productService.GetProductByIdAsync(productId);
            if (product != null)
            {
                ProductId = product.Id;
                Name = product.Name;
                Sku = product.SKU;
                Barcode = product.Barcode;
                ProductType = product.Type;
                BrandId = product.BrandId;
                CategoryId = product.CategoryId;
                SubCategoryId = product.SubCategoryId;
                UnitId = product.UnitId;
                CostPrice = product.CostPrice;
                SellingPrice = product.SellingPrice;
                ProfitMargin = product.ProfitMargin;
                AlertQuantity = product.AlertQuantity;
                Description = product.Description;
                ImagePath = product.ImagePath;
                IsActive = product.IsActive;
                HasExpiryDate = product.HasExpiryDate;
            }
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    public async Task SaveAsync()
    {
        if (!ValidateForm())
        {
            await _dialogService.ShowMessageAsync("Validation Error", "Please fix the errors before saving");
            return;
        }

        IsSaving = true;
        try
        {
            var product = new Entities.Product
            {
                Id = ProductId ?? 0,
                Name = Name,
                SKU = Sku,
                Barcode = Barcode,
                Type = ProductType,
                BrandId = BrandId,
                CategoryId = CategoryId,
                SubCategoryId = SubCategoryId,
                UnitId = UnitId,
                CostPrice = CostPrice,
                SellingPrice = SellingPrice,
                ProfitMargin = ProfitMargin,
                AlertQuantity = AlertQuantity,
                Description = Description,
                ImagePath = ImagePath,
                IsActive = IsActive,
                HasExpiryDate = HasExpiryDate,
                BusinessId = _sessionService.CurrentBusiness?.Id ?? 0
            };

            if (IsEditMode)
            {
                await _productService.UpdateProductAsync(product);
                await _dialogService.ShowMessageAsync("Success", "Product updated successfully");
            }
            else
            {
                await _productService.CreateProductAsync(product);
                await _dialogService.ShowMessageAsync("Success", "Product created successfully");
            }

            // TODO: Navigate back to product list
        }
        catch (Exception ex)
        {
            await _dialogService.ShowMessageAsync("Error", $"Failed to save product: {ex.Message}");
        }
        finally
        {
            IsSaving = false;
        }
    }

    [RelayCommand]
    public void Cancel()
    {
        // TODO: Navigate back to product list
    }

    public void SetImage(byte[] imageData, string imagePath)
    {
        ImageData = imageData;
        ImagePath = imagePath;
        HasImage = true;
    }

    [RelayCommand]
    public void ClearImage()
    {
        ImageData = null;
        ImagePath = null;
        HasImage = false;
    }

    private bool ValidateForm()
    {
        bool isValid = true;

        // Name validation
        if (string.IsNullOrWhiteSpace(Name))
        {
            NameError = "Product name is required";
            isValid = false;
        }
        else if (Name.Length > 200)
        {
            NameError = "Product name must be 200 characters or less";
            isValid = false;
        }
        else
        {
            NameError = null;
        }

        // SKU validation
        if (!string.IsNullOrWhiteSpace(Sku) && Sku.Length > 100)
        {
            SkuError = "SKU must be 100 characters or less";
            isValid = false;
        }
        else
        {
            SkuError = null;
        }

        // Cost price validation
        if (CostPrice < 0)
        {
            CostPriceError = "Cost price cannot be negative";
            isValid = false;
        }
        else
        {
            CostPriceError = null;
        }

        // Selling price validation
        if (SellingPrice < 0)
        {
            SellingPriceError = "Selling price cannot be negative";
            isValid = false;
        }
        else
        {
            SellingPriceError = null;
        }

        return isValid;
    }
}
