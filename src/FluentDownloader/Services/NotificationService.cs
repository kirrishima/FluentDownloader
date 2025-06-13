using Microsoft.Windows.AppNotifications.Builder;
using Microsoft.Windows.AppNotifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Notifications;
using Windows.UI.Popups;
using System.Security;
using System.Diagnostics;
using FluentDownloader.Helpers;
using Microsoft.UI.Xaml;
using WinRT.Interop;
using Microsoft.UI.Xaml.Controls;

namespace FluentDownloader.Services;

public static class NotificationService
{
    public static void ShowNotification(string title, string message)
    {
        if (!IsSupported())
            return;
        try
        {
            string xmlPayload = $@"
                <toast>
                    <visual>
                        <binding template='ToastGeneric'>
                            <text>{title}</text>
                            <text>{message}</text>
                        </binding>
                    </visual> {(App.AppSettings.Notifications.EnableSoundWhenPopup ? string.Empty : "<audio silent=\"\"true\"\"/>")}
                    {@"        <actions>
            <action 
                content="""""" 
                arguments=""""dismiss"""" 
                activationType=""""background""""/>
        </actions>"}
                </toast>";

            var toast = new AppNotification(xmlPayload);
            AppNotificationManager.Default.Show(toast);
        }
        catch
        {
        }
    }

    public enum ImageSize
    {
        Hero,
        Inline,
        AppLogoOverride
    }

    public static void ShowNotificationWithImage(string title, string message, string? imageUrl)
    {
        try
        {
            if (!IsSupported())
                return;

            string audioTag = App.AppSettings.Notifications.EnableSoundWhenPopup
            ? string.Empty
            : "<audio silent='true'/>";

            string image = !App.AppSettings.Notifications.ShowImageAtNotifications
                ? string.Empty
                : $@"<image 
                        src='{SecurityElement.Escape(imageUrl)}' 
                        placement='{App.AppSettings.Notifications.ImageInNotificationSize}'
                        hint-crop='rounded'/>";

            string xmlPayload = $@"
                        <toast launch='action=toast_click'>
                            <visual>
                                <binding template='ToastGeneric'>
                                    <text>{SecurityElement.Escape(title)}</text>
                                    <text>{SecurityElement.Escape(message)}</text>
                                    {image}
                                </binding>
                            </visual>
                            {audioTag}
                        </toast>";
            // Создание и отображение уведомления
            var toast = new AppNotification(xmlPayload);
            AppNotificationManager.Default.Show(toast);

        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Ошибка при отображении уведомления: {ex.Message}");
        }
    }

    private static bool IsSupported()
    {
        try
        {
            if (App.AppSettings.Notifications.ShowOnlyIfTheWindowIsInactive)
            {
                IntPtr hWnd = WindowNative.GetWindowHandle(App.MainWindow);

                bool isMinimized = WindowHelper.IsWindowMinimized(hWnd);
                bool isActive = WindowHelper.IsWindowActive(hWnd);

                if (isActive && !isMinimized)
                {
                    return false;
                }
            }

            if (!App.AppSettings.Notifications.EnableDesktopNotifications)
            {
                return false;
            }

            return AppNotificationManager.IsSupported();
        }
        catch
        {
            return false;
        }
    }

}
