using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace UltimatePOS.WinUI.Converters;

public class EmptyCountToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is int count)
        {
            return count <= 0 ? Visibility.Visible : Visibility.Collapsed;
        }
        return Visibility.Visible; // Default to visible if null or not int (empty state safe default)
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
