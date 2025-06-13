using Windows.Storage;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics;

namespace FluentDownloader.Settings;

public class WindowSettings : INotifyPropertyChanged
{
    private readonly ApplicationDataContainer _localSettings;

    public event PropertyChangedEventHandler? PropertyChanged;

    public WindowSettings(ApplicationDataContainer localSettings)
    {
        _localSettings = localSettings;
    }

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private SizeInt32? _windowSize;
    private bool _isWindowSizeLoaded = false;

    /// <summary>
    /// Gets or sets the window size saved in local settings.
    /// The value is stored as a string in the format "Width;Height".
    /// </summary>
    public SizeInt32? WindowSize
    {
        get
        {
            if (!_isWindowSizeLoaded)
            {
                if (_localSettings.Values.TryGetValue("WindowSize", out object? stored) &&
                    stored is string sizeString && !string.IsNullOrEmpty(sizeString))
                {
                    // Ожидается формат "Width;Height", например, "1024;768"
                    var parts = sizeString.Split(';');
                    if (parts.Length == 2 &&
                        int.TryParse(parts[0], out int width) &&
                        int.TryParse(parts[1], out int height))
                    {
                        _windowSize = new SizeInt32 { Width = width, Height = height };
                    }
                    else
                    {
                        _windowSize = null;
                    }
                }
                else
                {
                    _windowSize = null;
                }
                _isWindowSizeLoaded = true;
            }
            return _windowSize;
        }
        set
        {
            if (_isWindowSizeLoaded && _windowSize.Equals(value))
                return;

            _windowSize = value;
            _isWindowSizeLoaded = true;
            _localSettings.Values["WindowSize"] = value.HasValue ? $"{value.Value.Width};{value.Value.Height}" : null;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WindowSize)));
        }
    }

    public int WindowWidth
    {
        get => WindowSize?.Width ?? App.MainWindow.AppWindow.Size.Width;
        set
        {
            if (value <= 0)
            {
                WindowSize = null;
                return;
            }

            int currentHeight = WindowSize?.Height ?? 0;
            if (WindowSize == null || WindowSize.Value.Width != value)
            {
                WindowSize = new SizeInt32 { Width = value, Height = currentHeight };
                OnPropertyChanged(nameof(WindowWidth));
            }
        }
    }

    public int WindowHeight
    {
        get => WindowSize?.Height ?? App.MainWindow.AppWindow.Size.Height;
        set
        {
            if (value <= 0)
            {
                WindowSize = null;
                return;
            }

            int currentWidth = WindowSize?.Width ?? 0;
            if (WindowSize == null || WindowSize.Value.Height != value)
            {
                WindowSize = new SizeInt32 { Width = currentWidth, Height = value };
                OnPropertyChanged(nameof(WindowHeight));
            }
        }
    }

    /// <summary>
    /// </summary>
    // Приватное поле для кэширования значения
    private bool _startAtFullscreenMode;
    private bool _isStartAtFullscreenModeLoaded = false;

    public bool StartAtFullscreenMode
    {
        get
        {
            if (!_isStartAtFullscreenModeLoaded)
            {
                if (_localSettings.Values.TryGetValue("StartAtFullscreenMode", out object? value))
                {
                    _startAtFullscreenMode = (bool)value;
                }
                else
                {
                    _startAtFullscreenMode = false; // значение по умолчанию
                }
                _isStartAtFullscreenModeLoaded = true;
            }
            return _startAtFullscreenMode;
        }
        set
        {
            // Если значение не изменилось, ничего не делаем
            if (_isStartAtFullscreenModeLoaded && _startAtFullscreenMode == value)
                return;

            _startAtFullscreenMode = value;
            _isStartAtFullscreenModeLoaded = true;
            _localSettings.Values["StartAtFullscreenMode"] = value;
            // Если требуется уведомлять об изменении
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StartAtFullscreenMode)));
        }
    }
}
