using CommunityToolkit.WinUI;
using FluentDownloader;
using FluentDownloader.Dialogs;
using FluentDownloader.Helpers;
using FluentDownloader.Models;
using FluentDownloader.Services;
using FluentDownloader.Services.Dependencies.Helpers;
using FluentDownloader.Services.Dependencies.Installations;
using FluentDownloader.Services.Ytdlp;
using FluentDownloader.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.Windows.AppNotifications;
using Microsoft.Windows.AppNotifications.Builder;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.Globalization;
using Windows.Storage;
// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace FluentDownloader.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page, IDialogService, IDownloadDependencies, IProgressBar
    {
        public static MainPage? Instance { get; private set; }
        /// <summary>
        /// Service responsible for managing YT-DLP installation and updates.
        /// Handles dependency checks, installation prompts, and version validation.
        /// </summary>
        private YtdlpInstallerService _ytdlpInstallerService = null!;

        /// <summary>
        /// Service responsible for managing FFmpeg installation and updates.
        /// Coordinates with YT-DLP downloader for compatibility validation.
        /// </summary>
        private FfmpegInstallerService _ffmpegInstallerService = null!;

        /// <summary>
        /// Core download service handling video download operations using YT-DLP.
        /// Manages download progress, format selection, and metadata retrieval.
        /// </summary>
        private YtDlpDownloader ytDlpDownloader = null!;

        /// <summary>
        /// Timed property updater for managing UI element states and progress tracking.
        /// Coordinates periodic updates for download progress and status indicators.
        /// </summary>
        private TimedPropertyUpdater<string> propertyUpdater = null!;

        /// <summary>
        /// Semaphore for managing concurrent access to dialog operations.
        /// Ensures thread-safe presentation of modal dialogs and user prompts.
        /// </summary>
        private static readonly SemaphoreSlim dialogSemaphore = new(1, 1);

        /// <summary>
        /// Token source for managing cancellation of ongoing download operations.
        /// Enables cooperative cancellation of active downloads and background tasks.
        /// </summary>
        private CancellationTokenSource DownloadCts { get; set; } = null!;

        /// <summary>
        /// Gets the progress bar control used to visualize download progress.
        /// Linked directly to the XAML-defined DownloadingProgressBar UI element.
        /// </summary>
        public ProgressBar ProgressBar { get => DownloadingProgressBar; }

        /// <summary>
        /// Flag indicating whether window initialization has completed.
        /// Used to prevent duplicate initialization operations.
        /// </summary>
        /// <value>
        /// <c>true</c> if initialization finalized; <c>false</c> otherwise.
        /// </value>
        private bool _initialized = false;

        /// <summary>
        /// Gets or sets the text block style used for popup UI elements.
        /// Provides consistent typography and formatting for informational popups.
        /// </summary>
        /// <value>
        /// A <see cref="Style"/> resource targeting <see cref="TextBlock"/> elements.
        /// May be null until styles are initialized.
        /// </value>
        private Style PopUpsTextBlockStyle { get; set; } = null!;

        /// <summary>
        /// Tooltip control associated with the download button.
        /// Provides contextual help and operational guidance for the download feature.
        /// </summary>
        /// <remarks>
        /// Initialized dynamically during UI setup phase.
        /// </remarks>
        private ToolTip _DownloadButtonToolTip = null!;

        public DownloadQueueViewModel DownloadQueueViewModel { get; private set; } = null!;

        public VideoDownloadViewModel VideoDownloadViewModel { get; private set; } = new();

        public MainPage()
        {
            if (_initialized) return;
            Instance = this;
            InitializeCoreComponents();
            this.NavigationCacheMode = Microsoft.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            InitializeUiComponents();
            this.Loaded += MainWindow_Activated;

            InitializeServices();
            InitializeViewModels();
            InitializeAsyncOperations();
            FinalizeInitialization();
            ConfigureSystemBackdrop();

            this.SizeChanged += MainPage_SizeChanged;

            DataContext = this;
        }

        bool _isSmallView = false;

        private void MainPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Width < 1200 && !_isSmallView)
            {
                _isSmallView = true;

                Grid.SetColumn(NotificationStack, 0);
                Grid.SetColumnSpan(NotificationStack, 4);
                Grid.SetColumn(DownloadQueue, 2);
                Grid.SetColumnSpan(DownloadQueue, 2);
                DownloadQueue.Margin = new(0, 0, 0, 50);

                Grid.SetColumn(RootContentGrid, 0);
                Grid.SetColumnSpan(RootContentGrid, 4);

                Grid.SetColumn(LeftSidebar, 0);
                Grid.SetRow(LeftSidebar, 2);
                Grid.SetColumnSpan(LeftSidebar, 3);
                LeftSidebar.Margin = new Thickness(0, 0, 0, 8);

                Grid.SetColumn(Header, 0);
                Grid.SetRow(Header, 0);
                Grid.SetColumnSpan(Header, 3);

                Grid.SetColumn(RootScrollViewer, 0);
                Grid.SetRow(RootScrollViewer, 1);
                Grid.SetColumnSpan(RootScrollViewer, 3);

                LeftSidebar.Orientation = Orientation.Horizontal;
                RootGrid.Padding = new Thickness(20, 0, 20, 0);
            }
            else if (e.NewSize.Width > 1200 && _isSmallView)
            {
                _isSmallView = false;
                Grid.SetColumn(NotificationStack, 3);
                Grid.SetColumnSpan(NotificationStack, 1);
                Grid.SetColumn(DownloadQueue, 3);
                Grid.SetColumnSpan(DownloadQueue, 1);
                DownloadQueue.Margin = new(0, 0, 0, 0);

                Grid.SetColumn(RootContentGrid, 0);
                Grid.SetColumnSpan(RootContentGrid, 3);

                Grid.SetColumn(LeftSidebar, 0);
                Grid.SetRow(LeftSidebar, 1);
                Grid.SetColumnSpan(LeftSidebar, 1);
                LeftSidebar.Margin = new Thickness(0, 0, 0, 0);

                Grid.SetColumn(Header, 1);
                Grid.SetRow(Header, 0);
                Grid.SetColumnSpan(Header, 2);

                Grid.SetColumn(RootScrollViewer, 1);
                Grid.SetRow(RootScrollViewer, 1);
                Grid.SetColumnSpan(RootScrollViewer, 2);

                RootGrid.Padding = new Thickness(20, 0, 20, 20);
                LeftSidebar.Orientation = Orientation.Vertical;
            }
        }


        /// <summary>
        /// Initializes fundamental application components and services.
        /// Creates property updater, application settings, and XAML components.
        /// </summary>
        private void InitializeCoreComponents()
        {
            // Basic component initialization
            propertyUpdater = new TimedPropertyUpdater<string>("Initial Value");
            //AppSettings = new AppSettings();
            this.InitializeComponent();
        }

        /// <summary>
        /// Configures UI elements and visual presentation settings.
        /// Sets up title bar integration, language selection, and theme initialization.
        /// </summary>
        private void InitializeUiComponents()
        {

        }

        /// <summary>
        /// Configures UI element styles with fallback handling.
        /// Applies specialized text block styling for popup elements, 
        /// using a default style if the primary style isn't found.
        /// </summary>
        /// <remarks>
        /// Logs style initialization progress through the property updater.
        /// Throws <see cref="InvalidOperationException"/> if both styles are missing.
        /// </remarks>
        private void InitializeUiStyles()
        {
            propertyUpdater.LogStep("Processing styles");
            PopUpsTextBlockStyle = StylesManager.GetStyleOrDefault("PopUpsTextBlock", "BodyTextBlockStyle");
        }

        [MemberNotNull(nameof(DownloadQueueViewModel))]
        private void InitializeViewModels()
        {
            var animator = new DownloadQueueAnimator(
              fadeInNotifications: (Storyboard)Resources["FadeInNotifications"],
              fadeOutNotifications: (Storyboard)Resources["FadeOutNotifications"],
              queuePanel: DownloadQueue,
              queueTransform: QueueTranslate,
              notificationPanel: NotificationStack,
              slideInQueue: (Storyboard)Resources["SlideInQueue"],
              slideOutQueue: (Storyboard)Resources["SlideOutQueue"]
              );

            DownloadQueueViewModel = new DownloadQueueViewModel(animator);
        }

        /// <summary>
        /// Initializes service dependencies and installer components.
        /// Creates YT-DLP/FFmpeg services with shared confirmation handlers.
        /// </summary>
        private void InitializeServices()
        {
            // Download service initialization
            ytDlpDownloader = new YtDlpDownloader(this, this, this);

            // Installer services with common confirmation handler
            var confirmationHandler = async (string content, string yes, string no)
            => await PromptUserAsync(null, content, yes, no);

            _ytdlpInstallerService = new(this, this, this, ytDlpDownloader, confirmationHandler);
            _ffmpegInstallerService = new(this, this, this, ytDlpDownloader, confirmationHandler);
        }

        /// <summary>
        /// Configures system backdrop effects based on current settings.
        /// Applies either acrylic or mica material based on user preference.
        /// </summary
        private void ConfigureSystemBackdrop()
        {
            // Background effect initialization

            App.EventAggregator.ChangeSystemBackdrop(this, new DesktopAcrylicBackdrop());
        }

        /// <summary>
        /// Initiates background initialization of download-related components.
        /// Coordinates asynchronous setup of download options and configurations.
        /// </summary>
        /// <remarks>
        /// Called during main window initialization phase.
        /// </remarks>
        private void InitializeAsyncOperations()
        {
            // Background initialization setup
            InitializeDownloadOptions();

        }

        /// <summary>
        /// Performs final initialization steps and marks window as ready.
        /// Completes property tracking setup and signals completion of initialization.
        /// </summary>
        private void FinalizeInitialization()
        {
            propertyUpdater.LogStep("Leave MainWindow()");
            _initialized = true;
        }

        /// <summary>
        /// Handles window activation event to complete asynchronous initialization.
        /// Triggers post-activation setup and detaches itself after first execution.
        /// </summary>
        /// <param name="sender">Event source</param>
        /// <param name="e">Window activation state data</param>
        private void MainWindow_Activated(object sender, RoutedEventArgs e)
        {
            propertyUpdater.LogStep("MainWindow_Activated");
            this.Loaded -= MainWindow_Activated;

            _ = InitializeAsync();
        }

        /// <summary>
        /// Enables interactive UI controls after successful initialization.
        /// Activates dependency info buttons and main download functionality.
        /// </summary>
        private void EnableUiControls()
        {
            FfmpegInfoButton.IsEnabled = true;
            YtDlpInfoButton.IsEnabled = true;
            VideoDownloadViewModel.YtdlpServiceIsBusy = true;
        }


        /// <summary>
        /// Performs complete application initialization sequence.
        /// Coordinates settings loading, dependency checks, and UI finalization.
        /// </summary>
        /// <returns>Task representing asynchronous initialization process</returns>
        private async Task InitializeAsync()
        {
            try
            {
                propertyUpdater.LogStep("InitializeAsync");
                SavePathTextBox.Text = App.AppSettings.General.LastPeekedOutputPath;

                if (string.IsNullOrWhiteSpace(SavePathTextBox.Text))
                {
                    SavePathTextBox.Text = LocalizedStrings.GetMessagesString("SelectSavePathPlaceholder");
                }

                while (!this._contentLoaded || !this.IsLoaded) { await Task.Delay(100); }
                InitializeDownloadButtonToolTip();

                propertyUpdater.LogStep("InitializeAsync _contentLoaded = true");

                await LoadDependenciesAsync();
                await CheckForDependencies();

                DispatcherQueue.TryEnqueue(() =>
                {
                    EnableUiControls();
                });

                propertyUpdater.LogStep("Leaving InitializeAsync");
            }
            catch (Exception ex)
            {
                propertyUpdater.LogStep("InitializeAsync catch");
                await ShowNotificationDialogAsync("Error", ex.Message);
                return;
            }
        }

        /// <summary>
        /// Initializes the tooltip for the download button.
        /// </summary>
        private void InitializeDownloadButtonToolTip()
        {
        }

        private void SwitchPageButton_Click(object sender, RoutedEventArgs e)
        {
            App.EventAggregator.ChangePage(typeof(StyleEditorPage));
        }

        private void GoToSettingsPageButton_Click(object sender, RoutedEventArgs e)
        {
            App.EventAggregator.ChangePage(typeof(SettingsPage));
        }
    }
}
