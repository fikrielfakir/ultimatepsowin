using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace UltimatePOS.WinUI.Converters;

public class InverseBooleanToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is bool booleanValue)
        {
            return !booleanValue ? Visibility.Visible : Visibility.Collapsed;
        }
        // If not boolean, assume false (so return Visible)? Or collapsed? Usually false.
        // But if null, maybe Visible?
        // Let's stick to standard reversed logic: false -> Visible, true -> Collapsed.
        return Visibility.Visible; 
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        return value is Visibility visibility && visibility == Visibility.Collapsed;
    }
}
