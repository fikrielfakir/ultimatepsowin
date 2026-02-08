using System;
using Microsoft.UI.Xaml.Data;

namespace UltimatePOS.WinUI.Converters;

public class DateTimeToStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is DateTime dateTime)
        {
            return dateTime.ToString("g"); // General date/time pattern (short time)
        }
        if (value is DateTimeOffset dateTimeOffset)
        {
            return dateTimeOffset.ToString("g");
        }
        return string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
