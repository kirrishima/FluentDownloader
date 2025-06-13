using Windows.Storage;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Windows.AppNotifications;
using FluentDownloader.Services;

namespace FluentDownloader.Settings;

public class NotificationSettings : INotifyPropertyChanged
{
    private readonly ApplicationDataContainer _localSettings;

    public event PropertyChangedEventHandler? PropertyChanged;

    public NotificationSettings(ApplicationDataContainer localSettings)
    {
        _localSettings = localSettings;
    }

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private bool _enableSoundWhenPopup;
    private bool _isEnableSoundWhenPopupLoaded = false;

    public bool EnableSoundWhenPopup
    {
        get
        {
            if (!_isEnableSoundWhenPopupLoaded)
            {
                if (_localSettings.Values.TryGetValue("EnableSoundInDesktopNotificationWhenPopup", out object? value))
                {
                    _enableSoundWhenPopup = (bool)value;
                }
                else
                {
                    _enableSoundWhenPopup = false; // значение по умолчанию
                }
                _isEnableSoundWhenPopupLoaded = true;
            }
            return _enableSoundWhenPopup;
        }
        set
        {
            // Если значение не изменилось, ничего не делаем
            if (_isEnableSoundWhenPopupLoaded && _enableSoundWhenPopup == value)
                return;

            _enableSoundWhenPopup = value;
            _isEnableSoundWhenPopupLoaded = true;
            _localSettings.Values["EnableSoundInDesktopNotificationWhenPopup"] = value;
            // Если требуется уведомлять об изменении
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EnableSoundWhenPopup)));
        }
    }

    private bool _enableDesktopNotifications;
    private bool _isEnableDesktopNotificationsLoaded = false;

    public bool EnableDesktopNotifications
    {
        get
        {
            if (!_isEnableDesktopNotificationsLoaded)
            {
                if (_localSettings.Values.TryGetValue("EnableDesktopNotifications", out object? value))
                {
                    _enableDesktopNotifications = (bool)value;
                }
                else
                {
                    _enableDesktopNotifications = AppNotificationManager.IsSupported(); // значение по умолчанию
                }
                _isEnableDesktopNotificationsLoaded = true;
            }
            return _enableDesktopNotifications;
        }
        set
        {
            if (_isEnableDesktopNotificationsLoaded && _enableDesktopNotifications == value)
                return;

            _enableDesktopNotifications = value;
            _isEnableDesktopNotificationsLoaded = true;
            _localSettings.Values["EnableDesktopNotifications"] = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EnableDesktopNotifications)));
        }
    }

    private bool _showImageAtNotifications;
    private bool _isShowImageAtNotificationsLoaded = false;

    public bool ShowImageAtNotifications
    {
        get
        {
            if (!_isShowImageAtNotificationsLoaded)
            {
                if (_localSettings.Values.TryGetValue("ShowImageAtNotifications", out object? value))
                {
                    _showImageAtNotifications = (bool)value;
                }
                else
                {
                    _showImageAtNotifications = true;
                }
                _isShowImageAtNotificationsLoaded = true;
            }
            return _showImageAtNotifications;
        }
        set
        {
            if (_isShowImageAtNotificationsLoaded && _showImageAtNotifications == value)
                return;

            _showImageAtNotifications = value;
            _isShowImageAtNotificationsLoaded = true;
            _localSettings.Values["ShowImageAtNotifications"] = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ShowImageAtNotifications)));
        }
    }

    private NotificationService.ImageSize _imageInNotificationSize;
    private bool _isImageInNotificationSizeLoaded = false;

    public NotificationService.ImageSize ImageInNotificationSize
    {
        get
        {
            if (!_isImageInNotificationSizeLoaded)
            {
                if (_localSettings.Values.TryGetValue("ImageInNotificationSize", out object? value))
                {
                    _imageInNotificationSize = (NotificationService.ImageSize)value;
                }
                else
                {
                    _imageInNotificationSize = NotificationService.ImageSize.AppLogoOverride;
                }
                _isImageInNotificationSizeLoaded = true;
            }
            return _imageInNotificationSize;
        }
        set
        {
            if (_isImageInNotificationSizeLoaded && _imageInNotificationSize == value)
                return;

            _imageInNotificationSize = value;
            _isImageInNotificationSizeLoaded = true;
            _localSettings.Values["ImageInNotificationSize"] = (int)value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ImageInNotificationSize)));
        }
    }

    private bool _showOnlyIfTheWindowIsInactive;
    private bool _isShowOnlyIfTheWindowIsInactiveLoaded = false;

    public bool ShowOnlyIfTheWindowIsInactive
    {
        get
        {
            if (!_isShowOnlyIfTheWindowIsInactiveLoaded)
            {
                if (_localSettings.Values.TryGetValue("ShowDesktopNotificationOnlyIfTheWindowIsInactive", out object? value))
                {
                    _showOnlyIfTheWindowIsInactive = (bool)value;
                }
                else
                {
                    _showOnlyIfTheWindowIsInactive = true;
                }
                _isShowOnlyIfTheWindowIsInactiveLoaded = true;
            }
            return _showOnlyIfTheWindowIsInactive;
        }
        set
        {
            if (_isShowOnlyIfTheWindowIsInactiveLoaded && _showOnlyIfTheWindowIsInactive == value)
                return;

            _showOnlyIfTheWindowIsInactive = value;
            _isShowOnlyIfTheWindowIsInactiveLoaded = true;
            _localSettings.Values["ShowDesktopNotificationOnlyIfTheWindowIsInactive"] = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ShowOnlyIfTheWindowIsInactive)));
        }
    }

    private bool _openWindowWhenClicked;
    private bool _isOpenWindowWhenClickedLoaded = false;

    public bool OpenWindowWhenClicked
    {
        get
        {
            if (!_isOpenWindowWhenClickedLoaded)
            {
                if (_localSettings.Values.TryGetValue("OpenWindowWhenDesktopNotificationClicked", out object? value))
                {
                    _openWindowWhenClicked = (bool)value;
                }
                else
                {
                    _openWindowWhenClicked = true; // значение по умолчанию
                }
                _isOpenWindowWhenClickedLoaded = true;
            }
            return _openWindowWhenClicked;
        }
        set
        {
            if (_isOpenWindowWhenClickedLoaded && _openWindowWhenClicked == value)
                return;

            _openWindowWhenClicked = value;
            _isOpenWindowWhenClickedLoaded = true;
            _localSettings.Values["OpenWindowWhenDesktopNotificationClicked"] = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(OpenWindowWhenClicked)));
        }
    }
}
