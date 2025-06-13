using Microsoft.UI.Xaml;
using System;
using Windows.Storage;
using Windows.UI;
using System.Diagnostics;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI;
using Microsoft.UI.Xaml.Markup;
using FluentDownloader.Helpers;
using FluentDownloader.ViewModels;
using System.ComponentModel;
using CommunityToolkit.WinUI;
using CommunityToolkit.WinUI.Helpers;
using Windows.Graphics;
using System.Runtime.CompilerServices;
using Newtonsoft.Json.Linq;
using FluentDownloader.Models;

namespace FluentDownloader.Settings;

public class AppSettings : INotifyPropertyChanged
{
    private readonly ApplicationDataContainer _localSettings;

    public AppSettings()
    {
        _localSettings = ApplicationData.Current.LocalSettings;
        Appearance = new AppearanceSettings(_localSettings);
        Window = new WindowSettings(_localSettings);
        Notifications = new NotificationSettings(_localSettings);
        Download = new DownloadSettings(_localSettings);
        General = new GeneralSettings(_localSettings);
    }

    public AppearanceSettings Appearance { get; }
    public WindowSettings Window { get; }
    public NotificationSettings Notifications { get; }
    public DownloadSettings Download { get; }
    public GeneralSettings General { get; }

    // Общая реализация INPC
    public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}