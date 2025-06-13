using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI;

namespace FluentDownloader.Helpers
{
    /// <summary>
    /// Helper class to work with styles and brushes.
    /// Implements INotifyPropertyChanged for binding support.
    /// </summary>
    public class StylesManager : INotifyPropertyChanged
    {
        // Singleton instance
        private static StylesManager _instance = null!;
        public static StylesManager Instance => _instance ??= new StylesManager();

        private Brush? _windowBackgroundBrush;

        /// <summary>
        /// Gets or sets the background brush for the window.
        /// Notifies property changes when the brush is updated.
        /// </summary>
        public Brush? WindowBackgroundBrush
        {
            get => _windowBackgroundBrush;
            set
            {
                if (_windowBackgroundBrush != value)
                {
                    _windowBackgroundBrush = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Raises the PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">Name of the property that changed.</param>
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Retrieves a style resource with fallback to a default style if the primary style is missing.
        /// </summary>
        /// <param name="key">Primary style resource key.</param>
        /// <param name="defaultKey">Fallback style resource key.</param>
        /// <returns>The resolved Style object.</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when neither the primary style nor the fallback style is found.
        /// </exception>
        public static Style GetStyleOrDefault(string key, string defaultKey)
        {
            // Try to find the primary style
            if (Application.Current.Resources.TryGetValue(key, out var style) && style is Style foundStyle)
            {
                return foundStyle;
            }

            // If not found, try to find the fallback style
            if (Application.Current.Resources.TryGetValue(defaultKey, out var defaultStyle) && defaultStyle is Style fallbackStyle)
            {
                return fallbackStyle;
            }

            // If neither is found, throw an exception
            throw new InvalidOperationException($"Style not found for keys '{key}' and '{defaultKey}'.");
        }

        // Constant for the resource key of the window background brush.
        public const string WindowBackgroundBrushName = "DynamicAcrylicBrush";

        private const string DefaultWindowBackgroundBrushKey = "ApplicationPageBackgroundThemeBrush";

        public static Brush? GetDefaultBackground()
        {
            try
            {
                return Application.Current.Resources[DefaultWindowBackgroundBrushKey] as Brush;
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleGeneralException(ex);
                return null;
            }
        }

        /// <summary>
        /// Retrieves a brush to be used as the window background.
        /// If an acrylic brush is enabled in settings, returns the loaded acrylic brush.
        /// Otherwise, falls back to a default transparent brush.
        /// </summary>
        /// <returns>The Brush for window background, or null on error.</returns>
        public static Brush? GetWindowBackgroundBrush()
        {
            try
            {
                // Check if the acrylic brush is enabled in the settings.
                if (App.AppSettings.Appearance.UseCustomAcrylicBrush)
                {
                    var brush = App.AppSettings.Appearance.CustomAcrylicBrush;
                    if (brush is not null)
                    {
                        return brush;
                    }
                }
                if (App.AppSettings.Appearance.BackdropMode == ViewModels.BackdropMode.Acrylic)
                {
                    return null;
                }
                // Fallback to the default transparent system brush.
                var defaultBrush = (SolidColorBrush)Application.Current.Resources["SystemControlTransparentBrush"];
                if (defaultBrush is not null)
                {
                    return defaultBrush;
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Darkens a given color by converting it to HSL, reducing the lightness, and converting back to RGB.
        /// </summary>
        /// <param name="color">The original color.</param>
        /// <param name="darkenFactor">A factor (between 0 and 1) to darken the color.</param>
        /// <returns>The darkened color.</returns>
        public static Color DarkenColor(Color color, double darkenFactor)
        {
            // Convert RGB to HSL
            double r = color.R / 255.0;
            double g = color.G / 255.0;
            double b = color.B / 255.0;

            double max = Math.Max(r, Math.Max(g, b));
            double min = Math.Min(r, Math.Min(g, b));
            double h, s, l = (max + min) / 2;

            if (max == min)
            {
                // No saturation if all components are equal.
                h = s = 0;
            }
            else
            {
                double d = max - min;
                s = l > 0.5 ? d / (2 - max - min) : d / (max + min);

                if (max == r)
                    h = (g - b) / d + (g < b ? 6 : 0);
                else if (max == g)
                    h = (b - r) / d + 2;
                else
                    h = (r - g) / d + 4;

                h /= 6;
            }

            // Darken the lightness value
            l = Math.Max(0, l * darkenFactor);

            // Convert HSL back to RGB
            double q = l < 0.5 ? l * (1 + s) : l + s - l * s;
            double p = 2 * l - q;

            double hueToRgb(double t)
            {
                if (t < 0) t += 1;
                if (t > 1) t -= 1;
                if (t < 1 / 6.0) return p + (q - p) * 6 * t;
                if (t < 1 / 2.0) return q;
                if (t < 2 / 3.0) return p + (q - p) * (2 / 3.0 - t) * 6;
                return p;
            }

            r = hueToRgb(h + 1 / 3.0);
            g = hueToRgb(h);
            b = hueToRgb(h - 1 / 3.0);

            // Construct the new color from the computed RGB components
            return Color.FromArgb(
                color.A,
                (byte)(r * 255),
                (byte)(g * 255),
                (byte)(b * 255)
            );
        }

        /// <summary>
        /// Adjusts the brightness of the given color by the specified adjustment value.
        /// A positive adjustment makes the color lighter; a negative adjustment makes it darker.
        /// </summary>
        /// <param name="color">The base color.</param>
        /// <param name="adjustment">The adjustment value for each color channel (e.g., 20 or -20).</param>
        /// <returns>A new Color with adjusted brightness.</returns>
        public static Color AdjustColor(Color color, int adjustment)
        {
            byte Adjust(byte channel)
            {
                int newValue = channel + adjustment;
                if (newValue < 0) newValue = 0;
                if (newValue > 255) newValue = 255;
                return (byte)newValue;
            }
            return Color.FromArgb(color.A, Adjust(color.R), Adjust(color.G), Adjust(color.B));
        }
    }
}
