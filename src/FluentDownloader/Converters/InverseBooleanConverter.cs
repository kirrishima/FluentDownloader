using Microsoft.UI.Xaml.Data;
using System;

namespace FluentDownloader.Converters;

public class InverseBooleanConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is bool boolValue)
            return !boolValue;
        return false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        if (value is bool boolValue)
            return !boolValue;
        return false;
    }
}
