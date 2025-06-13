using FluentDownloader.Extensions;
using FluentDownloader.Services;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.AppNotifications;
using System;
using Windows.ApplicationModel.Appointments;

namespace FluentDownloader.Pages.Settings;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class NotificationsPage : Page
{
    public bool IsSupported { get; private set; }

    private bool _initialized = false;

    public NotificationsPage()
    {
        this.InitializeComponent();
        DataContext = this;
        IsSupported = AppNotificationManager.IsSupported();

        //foreach (var item in Enum.GetValues(typeof(NotificationService.ImageSize)))
        //{
        //    ComboBoxItem comboBoxItem = new ComboBoxItem() { Content = item.ToString(), Tag = (int)item };

        //    NotificationImageSizeComboBox.Items.Add(comboBoxItem);
        //}

        //for (int i = 0; i < NotificationImageSizeComboBox.Items.Count; i++)
        //{
        //    if (true)
        //    {

        //    }
        //}

        NotificationImageSizeComboBox.PopulateComboBoxWithEnum(App.AppSettings.Notifications.ImageInNotificationSize);
        _initialized = true;
    }

    private void NotificationImageSizeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (!_initialized)
        {
            return;
        }

        App.AppSettings.Notifications.ImageInNotificationSize = (NotificationService.ImageSize)((NotificationImageSizeComboBox.SelectedItem as ComboBoxItem)?.Tag
            ?? NotificationService.ImageSize.Hero);
    }
}
