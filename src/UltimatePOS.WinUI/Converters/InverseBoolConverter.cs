using System;
using Microsoft.UI.Xaml.Data;

namespace UltimatePOS.WinUI.Converters;

public class InverseBoolConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is bool booleanValue)
        {
            return !booleanValue;
        }
        return false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        if (value is bool booleanValue)
        {
            return !booleanValue;
        }
        return false;
    }
}
