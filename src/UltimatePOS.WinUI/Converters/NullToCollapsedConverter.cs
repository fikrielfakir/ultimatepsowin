using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace UltimatePOS.WinUI.Converters;

public class NullToCollapsedConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value == null)
        {
            return Visibility.Collapsed;
        }
        if (value is string str && string.IsNullOrEmpty(str))
        {
            return Visibility.Collapsed;
        }
        return Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
