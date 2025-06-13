using CommunityToolkit.WinUI.Controls;
using FluentDownloader.Helpers;
using FluentDownloader.Helpers.FileSystem;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.IO;

namespace FluentDownloader.Pages.Settings
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DependenciesPage : Page
    {
        public DependenciesPage()
        {
            this.InitializeComponent();
        }

        private static void ToggleSettingsCard(SettingsCard? settingsCard, bool enabled)
        {
            if (settingsCard is not null)
            {
                settingsCard.IsEnabled = enabled;
            }
        }

        private void CheckYtdlpUpdatesCard_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CheckFFmpegUpdatesCard_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private async void ChooseYtdlpExeCard_Click(object sender, RoutedEventArgs e)
        {
            var settingsCard = sender as SettingsCard;
            ToggleSettingsCard(settingsCard, false);
            try
            {

                var file = await FilePicker.PickExeFileAsync(App.MainWindow);

                if (file != null && File.Exists(file.Path))
                {
                    App.AppSettings.Download.YtDlpExePath = file.Path;
                }
            }
            finally
            {
                ToggleSettingsCard(settingsCard, true);
            }
        }

        private async void UseDefaultYtdlpExe_Click_1(object sender, RoutedEventArgs e)
        {
            var settingsCard = sender as SettingsCard;
            ToggleSettingsCard(settingsCard, false);

            var main = MainPage.Instance;

            if (main != null)
            {
                await main.LoadYtDlpDependencyAsync();
            }

            ToggleSettingsCard(settingsCard, true);
        }

        private async void ChooseFfmpegExeCard_Click(object sender, RoutedEventArgs e)
        {
            var settingsCard = sender as SettingsCard;
            ToggleSettingsCard(settingsCard, false);

            try
            {
                var file = await FilePicker.PickExeFileAsync(App.MainWindow);

                if (file != null && File.Exists(file.Path))
                {
                    App.AppSettings.Download.FfmpegExePath = file.Path;
                }
            }
            finally
            {
                ToggleSettingsCard(settingsCard, true);
            }
        }
        private async void UseDefaultFfmpegExe_Click(object sender, RoutedEventArgs e)
        {
            var settingsCard = sender as SettingsCard;
            ToggleSettingsCard(settingsCard, false);

            var main = MainPage.Instance;

            if (main != null)
            {
                await main.LoadFfmpegDependencyAsync();
            }

            ToggleSettingsCard(settingsCard, true);
        }
    }
}
