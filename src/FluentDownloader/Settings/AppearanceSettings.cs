using CommunityToolkit.WinUI.Helpers;
using FluentDownloader.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Media;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using Windows.Storage;
using Windows.UI;

namespace FluentDownloader.Settings;

public class AppearanceSettings : INotifyPropertyChanged
{
    private readonly ApplicationDataContainer _localSettings;

    public event PropertyChangedEventHandler? PropertyChanged;

    public AppearanceSettings(ApplicationDataContainer localSettings)
    {
        _localSettings = localSettings;
    }

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private ElementTheme _appTheme;
    private bool _isAppThemeLoaded = false;

    /// <summary>
    /// Gets or sets the application theme.
    /// If not set, returns ElementTheme.Default.
    /// </summary>
    public ElementTheme AppTheme
    {
        get
        {
            if (!_isAppThemeLoaded)
            {
                if (_localSettings.Values.TryGetValue("Theme", out object? value) &&
                    value is int themeValue && Enum.IsDefined(typeof(ElementTheme), themeValue))
                {
                    _appTheme = (ElementTheme)themeValue;
                }
                else
                {
                    _appTheme = ElementTheme.Default;
                }
                _isAppThemeLoaded = true;
            }
            return _appTheme;
        }
        set
        {
            if (_isAppThemeLoaded && _appTheme == value)
                return;

            _appTheme = value;
            _isAppThemeLoaded = true;
            _localSettings.Values["Theme"] = (int)value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AppTheme)));
        }
    }

    private Color? _accentColor;
    private bool _isAccentColorLoaded = false;

    /// <summary>
    /// Gets or sets the application accent color.
    /// When set, the color is saved to local settings and can be converted from its string representation.
    /// </summary>
    public Color? AccentColor
    {
        get
        {
            if (!_isAccentColorLoaded)
            {
                if (_localSettings.Values.TryGetValue("AccentColor", out object? stored) &&
                    stored is string colorString && !string.IsNullOrEmpty(colorString))
                {
                    _accentColor = (Color)XamlBindingHelper.ConvertValue(typeof(Color), colorString);
                }
                else
                {
                    _accentColor = null;
                }
                _isAccentColorLoaded = true;
            }
            return _accentColor;
        }
        set
        {
            if (_isAccentColorLoaded && _accentColor == value)
                return;

            _accentColor = value;
            _isAccentColorLoaded = true;
            _localSettings.Values["AccentColor"] = value?.ToString();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AccentColor)));
        }
    }

    private BackdropMode _backdropMode;
    private bool _isBackdropModeLoaded = false;

    /// <summary>
    /// Gets or sets the backdrop mode used by the application.
    /// If not set, returns BackdropMode.Acrylic.
    /// </summary>
    public BackdropMode BackdropMode
    {
        get
        {
            if (!_isBackdropModeLoaded)
            {
                if (_localSettings.Values.TryGetValue("BackdropMode", out object? value) &&
                    value is int modeValue && Enum.IsDefined(typeof(BackdropMode), modeValue))
                {
                    _backdropMode = (BackdropMode)modeValue;
                }
                else
                {
                    _backdropMode = BackdropMode.Acrylic;
                }
                _isBackdropModeLoaded = true;
            }
            return _backdropMode;
        }
        set
        {
            if (_isBackdropModeLoaded && _backdropMode == value)
                return;

            _backdropMode = value;
            _isBackdropModeLoaded = true;
            _localSettings.Values["BackdropMode"] = (int)value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BackdropMode)));
        }
    }

    private bool _useCustomAcrylicBrush;
    private bool _isUseCustomAcrylicBrushLoaded = false;

    public bool UseCustomAcrylicBrush
    {
        get
        {
            if (!_isUseCustomAcrylicBrushLoaded)
            {
                if (_localSettings.Values.TryGetValue("UseCustomAcrylicBrush", out object? value))
                {
                    _useCustomAcrylicBrush = (bool)value;
                }
                else
                {
                    _useCustomAcrylicBrush = false; // значение по умолчанию
                }
                _isUseCustomAcrylicBrushLoaded = true;
            }
            return _useCustomAcrylicBrush;
        }
        set
        {
            if (_isUseCustomAcrylicBrushLoaded && _useCustomAcrylicBrush == value)
                return;

            _useCustomAcrylicBrush = value;
            _isUseCustomAcrylicBrushLoaded = true;
            _localSettings.Values["UseCustomAcrylicBrush"] = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UseCustomAcrylicBrush)));
        }
    }

    private AcrylicBrush? _customAcrylicBrush;
    private bool _isCustomAcrylicBrushLoaded = false;

    [JsonConverter(typeof(AcrylicBrushJsonConverter))]
    public AcrylicBrush? CustomAcrylicBrush
    {
        get
        {
            if (!_isCustomAcrylicBrushLoaded)
            {
                try
                {
                    // Ensure all necessary values exist in the settings.
                    if (!_localSettings.Values.ContainsKey($"CustomAcrylicBrush_TintOpacity") ||
                        !_localSettings.Values.ContainsKey($"CustomAcrylicBrush_Opacity") ||
                        !_localSettings.Values.ContainsKey($"CustomAcrylicBrush_Color"))
                    {
                        _customAcrylicBrush = null;
                        _isCustomAcrylicBrushLoaded = true;
                        return null;
                    }

                    // Retrieve the values.
                    var tintOpacity = (double)_localSettings.Values[$"CustomAcrylicBrush_TintOpacity"];
                    var opacity = (double)_localSettings.Values[$"CustomAcrylicBrush_Opacity"];
                    var colorStr = (string)_localSettings.Values[$"CustomAcrylicBrush_Color"];

                    // Parse the color from the ARGB hex string.
                    Color color = colorStr.ToColor();

                    // Create and return a new AcrylicBrush with the loaded settings.
                    _customAcrylicBrush = new AcrylicBrush
                    {
                        TintColor = color,
                        TintOpacity = tintOpacity,
                        Opacity = opacity
                    };
                }
                catch
                {
                    _customAcrylicBrush = null;
                }
                _isCustomAcrylicBrushLoaded = true;
            }
            return _customAcrylicBrush;
        }
        set
        {
            if (value is null)
                return;

            _customAcrylicBrush = value;
            _isCustomAcrylicBrushLoaded = true;

            _localSettings.Values[$"CustomAcrylicBrush_TintOpacity"] = value.TintOpacity;
            _localSettings.Values[$"CustomAcrylicBrush_Opacity"] = value.Opacity;
            _localSettings.Values[$"CustomAcrylicBrush_Color"] = value.TintColor.ToString();

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CustomAcrylicBrush)));
        }
    }
}
