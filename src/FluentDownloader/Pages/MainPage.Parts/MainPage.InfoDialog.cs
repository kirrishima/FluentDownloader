using FluentDownloader.Dialogs;
using FluentDownloader.Helpers.FileSystem;
using FluentDownloader.Models;
using FluentDownloader.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.Windows.ApplicationModel.Resources;
using System;
using System.Threading.Tasks;

namespace FluentDownloader.Pages
{
    partial class MainPage
    {
        private ResourceMap? DialogsResourceMap { get; set; }

        /// <summary>
        /// Retrieves a localized string from the Dialogs resource map.
        /// </summary>
        /// <param name="resourceKey">The key of the resource string to retrieve.</param>
        /// <returns>The localized string associated with the provided key, or an empty string if not found.</returns>
        private string GetDialogsString(string resourceKey)
        {
            if (DialogsResourceMap is null)
            {
                var resourceManager = new Microsoft.Windows.ApplicationModel.Resources.ResourceManager();
                DialogsResourceMap = resourceManager.MainResourceMap.GetSubtree("Dialogs");
            }
            // Указываем путь к ресурсам

            try
            {
                return DialogsResourceMap.GetValue(resourceKey).ValueAsString;
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Displays an informational dialog or executes an installation action based on the installation status.
        /// </summary>
        /// <param name="sender">The object that triggered the event.</param>
        /// <param name="e">Event arguments associated with the event.</param>
        /// <param name="title">The title of the dialog.</param>
        /// <param name="isInstalled">Indicates whether the software is installed.</param>
        /// <param name="version">The version of the software, if installed.</param>
        /// <param name="executablePath">The file path of the executable, if installed.</param>
        /// <param name="executableName">The name of the executable file.</param>
        /// <param name="githubLinkText">The display text for the GitHub link.</param>
        /// <param name="githubLink">The URL to the software's GitHub page.</param>
        /// <param name="installAction">An asynchronous action to install the software if it is not installed.</param>
        private async Task ShowInfoButton_Click(object sender, RoutedEventArgs e,
                                       string title,
                                       bool isInstalled,
                                       string? version,
                                       string? executablePath,
                                       string executableName,
                                       string githubLinkText,
                                       string githubLink,
                                       Func<Task> installAction)
        {
            if (isInstalled)
            {
                var dialogContent = new GenericInfoDialog
                {
                    DataContext = new GenericInfoDialogViewModel(
                        versionText: string.Format(GetDialogsString("DependencyInfoVersion"), version?.Trim()),
                        descriptionText: string.Format(GetDialogsString("DependencyInfoPath"), executableName, executablePath),
                        buttonText: GetDialogsString("DependencyInfoRevealInExplorer"),
                        isButtonVisible: !string.Equals(executableName, executablePath, StringComparison.OrdinalIgnoreCase),
                        buttonCommand: new RelayCommand(() => FileSystemHelper.OpenInFileExplorer(executablePath)),
                        githubLink: githubLink,
                        githubLinkText: githubLinkText
                    )
                };

                await ShowNotificationDialogAsync(title, dialogContent);
            }
            else
            {
                await installAction();
            }
        }

        /// <summary>
        /// Handles the click event for the FFmpeg info button, displaying information or prompting installation.
        /// </summary>
        /// <param name="sender">The object that triggered the event.</param>
        /// <param name="e">Event arguments associated with the event.</param>
        private async void FfmpegInfoButton_Click(object sender, RoutedEventArgs e)
        {
            await ShowInfoButton_Click(
                sender,
                e,
                title: GetDialogsString("FfmpegInfoTitle"),
                isInstalled: ytDlpDownloader.FfmpegInfo.IsInstalled,
                version: ytDlpDownloader.FfmpegInfo.Version,
                executablePath: ytDlpDownloader.FfmpegInfo.Path,
                executableName: DependenciesConstants.FfmpegExecutable,
                installAction: ParseFfmpegMissing,
                githubLink: DependenciesConstants.FfmpegGitHubPage,
                githubLinkText: GetDialogsString("FfmpegInfoGithubLinkText")
            );
        }

        /// <summary>
        /// Handles the click event for the yt-dlp info button, displaying information or prompting installation.
        /// </summary>
        /// <param name="sender">The object that triggered the event.</param>
        /// <param name="e">Event arguments associated with the event.</param>
        private async void YtDlpInfoButton_Click(object sender, RoutedEventArgs e)
        {
            await ShowInfoButton_Click(
                sender,
                e,
                title: GetDialogsString("YtdlpInfoTitle"),
                isInstalled: ytDlpDownloader.YtDlpInfo.IsInstalled,
                version: ytDlpDownloader.YtDlpInfo.Version,
                executablePath: ytDlpDownloader.YtDlpInfo.Path,
                executableName: DependenciesConstants.YtDlpExecutable,
                installAction: ParseYtDlpMissing,
                githubLink: DependenciesConstants.YtDlpGitHubPage,
                githubLinkText: GetDialogsString("YtdlpInfoGithubLinkText")
            );
        }

    }
}
