using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;
using System.Diagnostics;
using System.Globalization;

namespace FluentDownloader.Converters
{
    /// <summary>
    /// Костыль. Уже не используется, но пусть будет
    /// Converter that returns VerticalAlignment.Bottom when the current UI culture is English,
    /// and VerticalAlignment.Center when it is Russian. Other cultures default to Center.
    /// </summary>
    public class LanguageBasedVerticalAlignmentConverter : IValueConverter
    {
        /// <summary>
        /// Converts the current UI culture to a VerticalAlignment value.
        /// </summary>
        /// <param name="value">Ignored.</param>
        /// <param name="targetType">The target type (should be VerticalAlignment).</param>
        /// <param name="parameter">Optional parameter (ignored).</param>
        /// <param name="language">The language (can be used if needed, but this converter uses CurrentUICulture).</param>
        /// <returns>
        /// VerticalAlignment.Bottom if the current UI culture is English ("en"),
        /// VerticalAlignment.Center if the current UI culture is Russian ("ru"),
        /// otherwise VerticalAlignment.Center.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string currentLanguage = Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride;
            Debug.WriteLine($"\n{currentLanguage}\n");
            if (currentLanguage.Equals("en-US", StringComparison.OrdinalIgnoreCase))
            {
                return VerticalAlignment.Bottom;
            }
            else if (currentLanguage.Equals("ru-RU", StringComparison.OrdinalIgnoreCase))
            {
                return VerticalAlignment.Center;
            }
            else
            {
                return VerticalAlignment.Center;
            }
        }

        /// <summary>
        /// ConvertBack is not implemented.
        /// </summary>
        /// <param name="value">Ignored.</param>
        /// <param name="targetType">Ignored.</param>
        /// <param name="parameter">Ignored.</param>
        /// <param name="language">Ignored.</param>
        /// <returns>Throws a NotImplementedException.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
