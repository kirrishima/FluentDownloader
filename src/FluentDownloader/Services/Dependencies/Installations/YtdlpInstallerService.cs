using FluentDownloader.Dialogs;
using FluentDownloader.Models;
using FluentDownloader.Services.Dependencies.Helpers;
using FluentDownloader.Services.Ytdlp;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentDownloader.Helpers;

namespace FluentDownloader.Services.Dependencies.Installations;

public class YtdlpInstallerService
{
    private YtDlpDownloader _downloader;

    private Func<string, string, string, Task<bool>> _promptUser;

    private readonly IDialogService _dialogService;
    private readonly IDownloadDependencies _downloadDependencies;
    private readonly IProgressBar _progressBar;

    public YtdlpInstallerService(IDialogService dialogService,
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
    /// Installs yt-dlp to the specified installation directory.
    /// </summary>
    /// <param name="installPath">The directory path where yt-dlp should be installed. If the path is null, the installation will fail.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result is <c>true</c> if the installation was successful;
    /// otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    /// This method handles the installation of yt-dlp, including creating the necessary directories, downloading the executable,
    /// and verifying the installation. If any step fails, an appropriate error message is displayed to the user.
    /// </remarks>
    public async Task<bool> InstallYtDlp(string installPath)
    {
        try
        {
            if (installPath is null)
            {
                await _dialogService.ShowNotificationDialogAsync(
                    LocalizedStrings.GetMessagesString("InstallYtDlpNullPathTitle"),
                    LocalizedStrings.GetMessagesString("InstallYtDlpNullPathDescription")
                );
                return false;
            }

            installPath = Path.Combine(installPath, DependenciesConstants.YtDlpBaseFolder);

            if (!Directory.Exists(installPath))
            {
                Directory.CreateDirectory(installPath);
            }

            installPath = Path.Combine(installPath, DependenciesConstants.YtDlpDownloadFilename);

            bool isInstalled = await DependencyInstaller.InstallDependencyAsync(
                DependenciesConstants.YtDlpExecutable,
                DependenciesConstants.YtDlpVersionArgs,
                DependenciesConstants.YtDlpLink,
                installPath,
                promptUser: _promptUser,
                logMessage: _downloadDependencies.LogsTextBoxWriteLine,
                printProgress: _progressBar.UpdateInstallProgress
            );

            _downloadDependencies.LogsTextBoxWriteLine();

            if (isInstalled)
            {
                (string? version, bool isYtdlpInstalledSuccessfully) = await Task.Run(() =>
                {
                    string? version = DependencyChecker.IsProgramAvailable(
                        installPath,
                        DependenciesConstants.YtDlpVersionArgs,
                        out bool isYtdlpInstalledSuccessfully
                    )?.Trim();

                    return (version, isYtdlpInstalledSuccessfully);
                });

                if (isYtdlpInstalledSuccessfully)
                {
                    _downloader.YtDlpInfo.IsInstalled = true;
                    _downloader.YtDlpInfo.Path = installPath;
                    _downloader.YtDlpInfo.Version = version;
                    App.AppSettings.Download.YtDlpExePath = installPath;
                    _dialogService.AddPopUpNotification(
                        LocalizedStrings.GetMessagesString("InstallYtDlpSuccessTitle"),
                        string.Format(LocalizedStrings.GetMessagesString("InstallYtDlpSuccessDescription"), version, _downloader.YtDlpInfo.Path),
                        InfoBarSeverity.Success
                    );
                }
                else
                {
                    _dialogService.AddPopUpNotification(
                        LocalizedStrings.GetMessagesString("InstallYtDlpFailedTitle"),
                        string.Format(LocalizedStrings.GetMessagesString("InstallYtDlpFailedDescription"), installPath),
                        InfoBarSeverity.Error
                    );
                }

                return isYtdlpInstalledSuccessfully;
            }

            return isInstalled;
        }
        catch (Exception ex)
        {
            _dialogService.AddPopUpNotification(
                LocalizedStrings.GetMessagesString("InstallYtDlpFailedTitle"),
                ex.Message,
                InfoBarSeverity.Error
            );
            _progressBar.SetProgressBarError(true);
            return false;
        }
    }
}
