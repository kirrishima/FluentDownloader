using FluentDownloader.Dialogs;
using FluentDownloader.Models;
using FluentDownloader.Services.Dependencies.Helpers;
using FluentDownloader.Services.Ytdlp;
using Microsoft.UI.Xaml.Controls;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FluentDownloader.Helpers;

namespace FluentDownloader.Services.Dependencies.Installations
{
    public class FfmpegInstallerService
    {
        private YtDlpDownloader _downloader;

        private Func<string, string, string, Task<bool>> _promptUser;

        private readonly IDialogService _dialogService;
        private readonly IDownloadDependencies _downloadDependencies;
        private readonly IProgressBar _progressBar;

        public FfmpegInstallerService(
            IDialogService dialogService,
            IDownloadDependencies downloadDependencies,
            IProgressBar progressBar,
            YtDlpDownloader ytDlpDownloader,
            Func<string, string, string, Task<bool>> promptUser)
        {
            _downloader = ytDlpDownloader;
            _dialogService = dialogService;
            _downloadDependencies = downloadDependencies;
            _progressBar = progressBar;
            _promptUser = promptUser;
        }

        /// <summary>
        /// Installs FFmpeg to the specified installation directory.
        /// </summary>
        /// <param name="installPath">The directory path where FFmpeg should be installed. If the path is null, the installation will fail.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result is <c>true</c> if the installation was successful;
        /// otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>
        /// This method handles the installation of FFmpeg, including creating the necessary directories, downloading the executable (or archive),
        /// and verifying the installation. If any step fails, an appropriate error message is displayed to the user.
        /// </remarks>
        public async Task<bool> InstallFfmpeg(string installPath)
        {
            try
            {
                if (installPath is null)
                {
                    await _dialogService.ShowNotificationDialogAsync(
                        LocalizedStrings.GetMessagesString("InstallFfmpegNullPathTitle"),
                        LocalizedStrings.GetMessagesString("InstallFfmpegNullPathDescription")
                    );
                    return false;
                }

                installPath = Path.Combine(installPath, DependenciesConstants.FfmpegBaseFolder);

                if (!Directory.Exists(installPath))
                {
                    Directory.CreateDirectory(installPath);
                }

                bool isInstalled = await DependencyInstaller.InstallDependencyAsync(
                    DependenciesConstants.FfmpegExecutable,
                    DependenciesConstants.FfmpegVersionArgs,
                DependenciesConstants.FfmpegLink,
                    installPath,
                    promptUser: _promptUser,
                    logMessage: _downloadDependencies.LogsTextBoxWriteLine,
                    isArchive: true,
                    printProgress: _progressBar.UpdateInstallProgress
                );

                _downloadDependencies.LogsTextBoxWriteLine();

                if (isInstalled)
                {
                    (string? version, string? ffmpegPath, bool isFfmpegInstalledSuccessfully) = await Task.Run(() =>
                    {
                        var ffmpegPath = Directory.GetFiles(installPath, DependenciesConstants.FfmpegExecutable, SearchOption.AllDirectories).FirstOrDefault();

                        if (ffmpegPath is not null)
                        {
                            string? version = DependencyChecker.IsProgramAvailable(ffmpegPath, DependenciesConstants.FfmpegVersionArgs, out bool isFfmpegInstalledSuccessfully);
                            return (version, ffmpegPath, isFfmpegInstalledSuccessfully);
                        }
                        return (null, ffmpegPath ?? string.Empty, false);

                    });

                    if (version != null)
                    {
                        version = DependencyChecker.ExtractFfmpegVersion(version)?.Trim();
                    }

                    if (isFfmpegInstalledSuccessfully)
                    {
                        _downloader.FfmpegInfo.IsInstalled = true;
                        _downloader.FfmpegInfo.Path = ffmpegPath;
                        _downloader.FfmpegInfo.Version = version;
                        App.AppSettings.Download.FfmpegExePath = ffmpegPath;
                        _dialogService.AddPopUpNotification(
                           LocalizedStrings.GetMessagesString("InstallFfmpegSuccessTitle"),
                            string.Format(LocalizedStrings.GetMessagesString("InstallFfmpegSuccessDescription"), version, _downloader.YtDlpInfo.Path),
                            InfoBarSeverity.Success
                        );
                    }
                    else
                    {
                        _dialogService.AddPopUpNotification(
                              LocalizedStrings.GetMessagesString("InstallFfmpegFailedTitle"),
                              string.Format(LocalizedStrings.GetMessagesString("InstallFfmpegFailedDescription"), installPath),
                              InfoBarSeverity.Error
                          );
                    }

                    return isFfmpegInstalledSuccessfully;
                }

                return isInstalled;
            }
            catch (Exception ex)
            {
                _progressBar.SetProgressBarError(true);
                _dialogService.AddPopUpNotification(
                    LocalizedStrings.GetMessagesString("InstallFfmpegFailedTitle"),
                    ex.Message,
                    InfoBarSeverity.Error
                );
                return false;
            }
        }
    }
}
