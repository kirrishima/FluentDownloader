using FluentDownloader.Helpers;
using FluentDownloader.Pages;
using FluentDownloader.Services.Ytdlp.Models;
using FluentDownloader.ViewModels;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.Windows.AppNotifications;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Windows.ApplicationModel;
using Windows.Graphics;
using Windows.Graphics.Display;
using WinRT.Interop;

namespace FluentDownloader
{
    /// <summary>
    /// Represents the main application window implementing the IDialogService interface.
    /// Handles UI initialization, dependency management, and application lifecycle operations.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the MainWindow class, setting up themes, styles, and event handlers.
        /// </summary>
        public static ElementTheme Theme { get; private set; }

        public Frame? Frame { get; private set; }

        public MainWindow()
        {
            this.InitializeComponent();
            RootThemeElement.DataContext = StylesManager.Instance;
            SetupWindowSize();
            this.ExtendsContentIntoTitleBar = true;

            //App.EventAggregator.ThemeChanged += UpdateTheme;
            BackdropViewModel.Instance.PropertyChanged += BackdropViewModel_PropertyChanged;
            BackdropViewModel.Instance.UpdateSystemBackdrop();

            ThemeViewModel.Instance.PropertyChanged += ThemeViewModel_PropertyChanged;
            UpdateTheme(ThemeViewModel.Instance.CurrentTheme);

            App.EventAggregator.PageChangeRequested += ChangePage;
            App.EventAggregator.PageGoBackRequested += () =>
            {
                if (MainFrame.CanGoBack)
                {
                    MainFrame.GoBack();
                }
                else
                {
                    MainFrame.Navigate(typeof(MainPage));
                }
            };
            App.EventAggregator.RecreateMainFrameRequested += RecreateMainFrame;

            Frame = MainFrame;

            SetIcon();

            MainFrame.Navigate(typeof(MainPage));
            //MainFrame.Navigate(typeof(SettingsPage));
        }

        private void SetupWindowSize()
        {
            var newSize = App.AppSettings.Window.WindowSize;
            if (newSize is not null && !App.AppSettings.Window.StartAtFullscreenMode)
            {
                try
                {
                    this.AppWindow.Resize(newSize.Value);
                }
                catch (Exception ex)
                {
                    ExceptionHandler.HandleGeneralException(ex);
                }
            }
            else if (App.AppSettings.Window.StartAtFullscreenMode)
            {
                if (this.AppWindow.Presenter is OverlappedPresenter presenter)
                {
                    presenter.Maximize();
                }
            }
        }

        private void SetIcon()
        {
            IntPtr hWnd = WindowNative.GetWindowHandle(this);
            var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);
            var appWindow = AppWindow.GetFromWindowId(windowId);

            // ѕуть к .ico Ч в упакованном приложении можно брать из InstalledLocation,
            // в распакованном Ч из AppContext.BaseDirectory или абсолютного пути.
            string iconPath = System.IO.Path.Combine("Assets", "AppIcon.ico");

            appWindow.SetIcon(iconPath);
        }

        private void BackdropViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(BackdropViewModel.SystemBackdrop))
            {
                UpdateBackdrop();
            }
        }

        private void UpdateBackdrop()
        {
            if (this.SystemBackdrop != BackdropViewModel.Instance.SystemBackdrop)
            {
                this.SystemBackdrop = BackdropViewModel.Instance.SystemBackdrop;
            }
        }

        private void ChangePage(Type page)
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                if (MainFrame.Content?.GetType() != page)
                {
                    MainFrame.Navigate(page);
                }
            });
        }

        private void ThemeViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ThemeViewModel.CurrentTheme))
            {
                UpdateTheme(ThemeViewModel.Instance.CurrentTheme);
            }
        }

        public void UpdateTheme(ElementTheme theme)
        {
            RootThemeElement.RequestedTheme = theme;
            Theme = theme;
        }

        private void RecreateMainFrame()
        {
            MainFrame = new Frame();
            this.Content = MainFrame;
            MainFrame.Navigate(typeof(MainPage));
        }

        public bool SystemBackDrop => this.SystemBackDrop;
    }
}
