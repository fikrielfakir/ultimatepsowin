using Microsoft.UI.Xaml.Controls;
using UltimatePOS.Core.ViewModels.Stock;
using UltimatePOS.Core.Models;
using System.Linq;
using System;
using Microsoft.Extensions.DependencyInjection;

namespace UltimatePOS.WinUI.Dialogs;

public sealed partial class StockAdjustmentDialog : ContentDialog
{
    public StockAdjustmentViewModel ViewModel { get; }
    public StockAdjustmentType[] AvailableTypes { get; } = Enum.GetValues<StockAdjustmentType>();

    public StockAdjustmentDialog()
    {
        this.InitializeComponent();
        ViewModel = ((App)Microsoft.UI.Xaml.Application.Current).Services.GetService<StockAdjustmentViewModel>();
    }
}
