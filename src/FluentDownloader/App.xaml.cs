using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using FluentDownloader.Helpers;
using Microsoft.UI.Xaml;
using Windows.UI;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using System.Diagnostics;
using System.Text.Json;
using Windows.UI.ViewManagement;
using Microsoft.Windows.AppLifecycle;
using Microsoft.Windows.AppNotifications;
using System.Runtime.InteropServices;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Windowing;
using WinRT.Interop;
using FluentDownloader.Settings;
using FluentDownloader.Pages;
using System.Text;

namespace FluentDownloader
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        public static DispatcherQueue DispatcherQueue { get; private set; } = null!;
        public static Window MainWindow { get; private set; } = null!;

        public static EventAggregator EventAggregator { get; private set; } = new EventAggregator();

        public static AcrylicBrush DynamicAcrylicBrush { get; private set; } = new();

        /// <summary>
        /// Manages persistent application settings and user preferences.
        /// Handles storage/retrieval of theme settings, download paths, and dependency locations.
        /// </summary>
        public static AppSettings AppSettings { get; private set; } = new();

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();

            DispatcherQueue = DispatcherQueue.GetForCurrentThread();

            UnhandledException += (_, e) =>
            {
                e.Handled = true;
                ProcessException(e.Exception);
            };
        }

        public static string GetErrorMessage(Exception e)
        {
            if (e is null) return string.Empty;

            var stringBuilder = new StringBuilder()
                .AppendLine(e.GetType().FullName)
                .AppendLine(e.ToString());

            if (e.InnerException != null)
            {
                stringBuilder.AppendLine("InnerException:");
                stringBuilder.AppendLine(e.InnerException.ToString());
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Show error message in an appropriate way
        /// </summary>
        /// <param name="errorMessage"></param>
        public static void ShowErrorMessage(Exception ex)
        {
            if (App.MainWindow is not null)
            {
                App.DispatcherQueue?.TryEnqueue(() =>
                {

                    var window = new Window() { Title = "Fluent Downloader" };
                    window.Content = new UnhandledExceptionPage(ex);
                    window.Activate();

                });
            }
            else
            {
                var window = new Window() { Title = "Fluent Downloader" };
                window.Content = new UnhandledExceptionPage(ex);
                window.Activate();
            }
        }

        public static void ProcessException(Exception ex)
        {

            ShowErrorMessage(ex);
        }

        public static Color SystemAccentColor { get; private set; }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override async void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            try
            {
                var mainInstance = Microsoft.Windows.AppLifecycle.AppInstance.FindOrRegisterForKey("Main");

                AppNotificationManager notificationManager = AppNotificationManager.Default;
                notificationManager.NotificationInvoked += NotificationManager_NotificationInvoked;
                notificationManager.Register();

                var activatedArgs = Microsoft.Windows.AppLifecycle.AppInstance.GetCurrent().GetActivatedEventArgs();
                var activationKind = activatedArgs.Kind;
                if (activationKind == ExtendedActivationKind.AppNotification)
                {
                    HandleNotification((AppNotificationActivatedEventArgs)activatedArgs.Data);
                }

                if (!mainInstance.IsCurrent)
                {
                    //Redirect the activation (and args) to the "main" instance, and exit.
                    var activatedEventArgs = Microsoft.Windows.AppLifecycle.AppInstance.GetCurrent().GetActivatedEventArgs();

                    await mainInstance.RedirectActivationToAsync(activatedEventArgs);
                    Process.GetCurrentProcess().Kill();
                    return;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            try
            {
                if (App.Current.Resources.ContainsKey("SystemAccentColor"))
                {
                    try
                    {
                        SystemAccentColor = (Color)XamlBindingHelper.ConvertValue(typeof(Color), App.Current.Resources["SystemAccentColor"]);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Error setting SystemAccentColor: {ex.Message}");
                    }
                }
                else
                {
                    Debug.WriteLine("Warning: SystemAccentColor not found in resources.");
                }

                if (AppSettings.Appearance.AccentColor is not null)
                {
                    try
                    {
                        var newAccentColor = (Color)XamlBindingHelper.ConvertValue(typeof(Color), AppSettings.Appearance.AccentColor);

                        try { Current.Resources["SystemAccentColor"] = newAccentColor; }
                        catch (Exception ex) { Debug.WriteLine($"Error setting SystemAccentColor: {ex.Message}"); }

                        try { Current.Resources["SystemControlHighlightAccentBrush"] = new SolidColorBrush(newAccentColor); }
                        catch (Exception ex) { Debug.WriteLine($"Error setting SystemControlHighlightAccentBrush: {ex.Message}"); }

                        try { Current.Resources["SystemAccentColorDark1"] = StylesManager.AdjustColor(newAccentColor, -20); }
                        catch (Exception ex) { Debug.WriteLine($"Error setting SystemAccentColorDark1: {ex.Message}"); }

                        try { Current.Resources["SystemAccentColorDark2"] = StylesManager.AdjustColor(newAccentColor, -10); }
                        catch (Exception ex) { Debug.WriteLine($"Error setting SystemAccentColorDark2: {ex.Message}"); }

                        try { Current.Resources["SystemAccentColorDark3"] = newAccentColor; }
                        catch (Exception ex) { Debug.WriteLine($"Error setting SystemAccentColorDark3: {ex.Message}"); }

                        try { Current.Resources["SystemAccentColorLight1"] = StylesManager.AdjustColor(newAccentColor, +20); }
                        catch (Exception ex) { Debug.WriteLine($"Error setting SystemAccentColorLight1: {ex.Message}"); }

                        try { Current.Resources["SystemAccentColorLight2"] = StylesManager.AdjustColor(newAccentColor, +30); }
                        catch (Exception ex) { Debug.WriteLine($"Error setting SystemAccentColorLight2: {ex.Message}"); }

                        try { Current.Resources["SystemAccentColorLight3"] = StylesManager.AdjustColor(newAccentColor, +40); }
                        catch (Exception ex) { Debug.WriteLine($"Error setting SystemAccentColorLight3: {ex.Message}"); }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Error converting AccentColor: {ex.Message}");
                    }
                }
                else
                {
                    Debug.WriteLine("Warning: AppSettings.AccentColor is null.");
                }

                try { StylesManager.Instance.WindowBackgroundBrush = StylesManager.GetWindowBackgroundBrush(); }
                catch (Exception ex) { Debug.WriteLine($"Error setting WindowBackgroundBrush: {ex.Message}"); }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unexpected error: {ex.Message}");
            }
            //ApplicationView.PreferredLaunchViewSize = new Size { Width = 600, Height = 600 };
            //ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
            m_window = new MainWindow();
            MainWindow = m_window;
            m_window.Activate();
        }

        private Window? m_window;

        private void NotificationManager_NotificationInvoked(
            AppNotificationManager sender,
            AppNotificationActivatedEventArgs args)
        {
            try
            {
                // Проверяем аргументы основного действия
                bool isToastClicked = args.Arguments
                    .TryGetValue("action", out var action)
                    && action == "toast_click";

                // Проверяем настройку и тип клика
                if (App.AppSettings.Notifications.OpenWindowWhenClicked && isToastClicked)
                {
                    IntPtr hWnd = WindowNative.GetWindowHandle(m_window);
                    bool isMinimized = WindowHelper.IsWindowMinimized(hWnd);
                    bool isActive = WindowHelper.IsWindowActive(hWnd);

                    if (m_window != null && (isMinimized || !isActive))
                    {
                        WindowHelper.BringWindowToFront(m_window);
                    }
                }

                // Дополнительно: обработка других действий
                if (args.Arguments.TryGetValue("action", out var actionType))
                {
                    switch (actionType)
                    {
                        case "dismiss":
                            // Обработка закрытия
                            break;
                        case "button_click":
                            // Обработка нажатия на кнопку
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleGeneralException(ex);
            }
        }

        private void HandleNotification(AppNotificationActivatedEventArgs args)
        {
            try
            {
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleGeneralException(ex);
            }
        }
    }
}
