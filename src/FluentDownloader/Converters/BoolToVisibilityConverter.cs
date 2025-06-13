using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;

namespace FluentDownloader.Converters
{
    /// <summary>
    /// Converts a boolean value to a Visibility enumeration.
    /// </summary>
    public class BooleanToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Converts a boolean value to Visibility.
        /// </summary>
        /// <param name="value">The boolean value to convert.</param>
        /// <param name="targetType">The target type (not used).</param>
        /// <param name="parameter">Optional conversion parameter (not used).</param>
        /// <param name="language">The language information (not used).</param>
        /// <returns>Visibility.Visible if true; otherwise, Visibility.Collapsed.</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool boolValue)
            {
                return boolValue ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        /// <summary>
        /// Converts a Visibility value back to a boolean.
        /// </summary>
        /// <param name="value">The Visibility value to convert.</param>
        /// <param name="targetType">The target type (not used).</param>
        /// <param name="parameter">Optional conversion parameter (not used).</param>
        /// <param name="language">The language information (not used).</param>
        /// <returns>True if Visibility is Visible; otherwise, false.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is Visibility visibilityValue)
            {
                return visibilityValue == Visibility.Visible;
            }
            return false;
        }
    }
}
