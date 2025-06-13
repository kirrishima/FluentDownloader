using FluentDownloader.Helpers;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace FluentDownloader.Pages.Settings
{
    public sealed partial class HomePage : Page
    {
        SettingsPage? _parentPage;

        public HomePage()
        {
            this.InitializeComponent();
        }

        private void NavigateToDownloadSections_Click(object sender, RoutedEventArgs e)
        {
            _parentPage = _parentPage ?? this.FindParent<SettingsPage>();

            _parentPage?.NavigateTo(
                typeof(DownloadPage),
                LocalizedStrings.GetSettingsString("DownloadingSectionHeader/Text"),
                null
                );
        }

        private void NavigateToLaunchSections_Click(object sender, RoutedEventArgs e)
        {
            _parentPage = _parentPage ?? this.FindParent<SettingsPage>();

            _parentPage?.NavigateTo(
                typeof(LaunchPage),
                LocalizedStrings.GetSettingsString("LaunchSettingsSectionHeader"),
                null
                );
        }

        private void AppearanceSection_Click(object sender, RoutedEventArgs e)
        {
            _parentPage = _parentPage ?? this.FindParent<SettingsPage>();

            _parentPage?.NavigateTo(
                typeof(StyleEditorPage),
                LocalizedStrings.GetSettingsString("AppearanceSectionHeader/Text"),
                null
                );
        }

        private void YtdlpAndFFmpegSettingsCard_Click(object sender, RoutedEventArgs e)
        {
            _parentPage = _parentPage ?? this.FindParent<SettingsPage>();

            _parentPage?.NavigateTo(
                typeof(DependenciesPage),
                LocalizedStrings.GetSettingsString("YtdlpAndFFmpegSettingsCardBreadcrump"),
                null
                );
        }

        private void GoToDesktopNotificationsPage_Click(object sender, RoutedEventArgs e)
        {
            _parentPage = _parentPage ?? this.FindParent<SettingsPage>();

            _parentPage?.NavigateTo(
                typeof(NotificationsPage),
                LocalizedStrings.GetSettingsString("DesktopNotificationsPage"),
                null
                );
        }
    }
}
