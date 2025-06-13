using Microsoft.UI.Xaml.Data;
using System;

namespace FluentDownloader.Converters;

public class NullToBooleanConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        return value != null;
    }

    public object? ConvertBack(object value, Type targetType, object parameter, string language)
    {
        // Обычно не используется, но можно вернуть null при значении false, если необходимо.
        return (value is bool boolValue && !boolValue) ? null : true;
    }
}

public class InverseNullToBooleanConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        return value == null;
    }

    public object? ConvertBack(object value, Type targetType, object parameter, string language)
    {
        // Обычно не используется, но можно вернуть null при значении false, если необходимо.
        return (value is bool boolValue && !boolValue) ? true : null;
    }
}

