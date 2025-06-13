using FluentDownloader.Helpers;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;
using System.Diagnostics;

namespace FluentDownloader.Converters
{
    public class ThemeToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is ElementTheme theme)
            {
                return LocalizedStrings.GetLeftSidebarString($"SwitchThemeTextBlock{theme}");
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
