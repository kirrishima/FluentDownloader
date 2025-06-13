using CommunityToolkit.WinUI;
using FluentDownloader.Dialogs.InstallDialogs;
using FluentDownloader.Models;
using FluentDownloader.Services.Dependencies.Helpers;
using FluentDownloader.Services.Dependencies.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;

namespace FluentDownloader.Pages
{
    public partial class MainPage
    {
        /// <summary>
        /// Handles the scenario where FFmpeg is missing by prompting the user to install it.
        /// Displays an installation dialog and, if the user agrees, initiates the FFmpeg installation process.
        /// </summary>
        private async Task ParseFfmpegMissing()
        {
            var isUserAgree = await ShowInstallDialogAsync(null, new InstallFfmpegDialog());

            if (isUserAgree)
            {
                await _ffmpegInstallerService.InstallFfmpeg(ApplicationData.Current.LocalFolder.Path);
            }
        }

        /// <summary>
        /// Handles the scenario where YT-DLP is missing by prompting the user to install it.
        /// Displays an installation dialog and, if the user agrees, initiates the YT-DLP installation process.
        /// </summary>
        private async Task ParseYtDlpMissing()
        {
            var isUserAgree = await ShowInstallDialogAsync(null, new InstallYtDlpDialog());

            if (isUserAgree)
            {
                await _ytdlpInstallerService.InstallYtDlp(ApplicationData.Current.LocalFolder.Path);
            }
        }

        /// <summary>
        /// Handles the scenario where both YT-DLP and FFmpeg are missing by prompting the user to install them.
        /// Displays an installation dialog and, if the user agrees, initiates the installation of both dependencies.
        /// </summary>
        private async Task ParseYtDlpAndFfmpegMissing()
        {

            var dialog = await DispatcherQueue.EnqueueAsync(() => new InstallYtDlpAndFfmpegDialog());
            var isUserAgree = await ShowInstallDialogAsync(null, dialog);

            if (isUserAgree)
            {
                await _ytdlpInstallerService.InstallYtDlp(ApplicationData.Current.LocalFolder.Path);
                await _ffmpegInstallerService.InstallFfmpeg(ApplicationData.Current.LocalFolder.Path);
            }
        }

        /// <summary>
        /// Checks for the presence of required dependencies (YT-DLP and FFmpeg) and prompts the user to install any missing components.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result is <c>true</c> if both dependencies are installed; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>
        /// This method evaluates the installation status of YT-DLP and FFmpeg and triggers the appropriate installation flow:
        /// - If both dependencies are missing, it prompts the user to install both.
        /// - If only FFmpeg is missing, it prompts the user to install FFmpeg.
        /// - If only YT-DLP is missing, it prompts the user to install YT-DLP.
        /// </remarks>
        private async Task<bool> CheckForDependencies()
        {
            return await DispatcherQueue.EnqueueAsync(async () =>
             {
                 if (!ytDlpDownloader.YtDlpInfo.IsInstalled && !ytDlpDownloader.FfmpegInfo.IsInstalled)
                 {
                     await ParseYtDlpAndFfmpegMissing();
                 }
                 else if (!ytDlpDownloader.FfmpegInfo.IsInstalled && ytDlpDownloader.YtDlpInfo.IsInstalled)
                 {
                     await ParseFfmpegMissing();
                 }
                 else if (ytDlpDownloader.FfmpegInfo.IsInstalled && !ytDlpDownloader.YtDlpInfo.IsInstalled)
                 {
                     await ParseYtDlpMissing();
                 }

                 return ytDlpDownloader.YtDlpInfo.IsInstalled && ytDlpDownloader.FfmpegInfo.IsInstalled;
             });
        }


        /// <summary>
        /// Loads and verifies dependency executables in background.
        /// Checks installed versions of YT-DLP and FFmpeg, validating paths and availability.
        /// </summary>
        /// <returns>Task representing asynchronous dependency verification</returns>
        private async Task LoadDependenciesAsync()
        {
            await Task.WhenAll(LoadYtDlpDependencyAsync(), LoadFfmpegDependencyAsync());
        }


        /// <summary>
        /// Обобщённый метод для загрузки и проверки зависимости.
        /// </summary>
        /// <param name="storedPath">Сохранённый путь из настроек.</param>
        /// <param name="defaultExecutable">Имя файла зависимости по умолчанию.</param>
        /// <param name="candidateNames">Список имён кандидатов для поиска.</param>
        /// <param name="versionArgs">Аргументы для получения версии.</param>
        /// <param name="versionProcessor">
        /// Функция для обработки строки версии (например, для ffmpeg можно извлечь только нужную часть),
        /// если дополнительная обработка не нужна, можно передать null.
        /// </param>
        /// <returns>Объект с информацией о зависимости.</returns>
        private async Task<DependencyInfo> LoadDependencyAsync(
            string? storedPath,
            string defaultExecutable,
            IEnumerable<string> candidateNames,
            string versionArgs,
            Func<string, string>? versionProcessor = null)
        {
            bool isAvailable = false;
            string? version = null;
            string chosenCandidate = "";

            await Task.Run(() =>
            {
                // 1. Если сохранённый путь указан и файл существует, проверяем его.
                if (!string.IsNullOrEmpty(storedPath) &&
                    (File.Exists(storedPath) || string.Equals(storedPath, defaultExecutable, StringComparison.OrdinalIgnoreCase)))
                {
                    version = DependencyChecker.IsProgramAvailable(storedPath, versionArgs, out bool available);
                    if (available)
                    {
                        isAvailable = true;
                        chosenCandidate = storedPath;
                    }
                }

                // 2. Если сохранённый путь не сработал, ищем кандидатов.
                if (!isAvailable)
                {
                    var candidatePaths = new List<string>();

                    // Поиск в локальной директории (со всеми подпапками)
                    string localFolderPath = ApplicationData.Current.LocalFolder.Path;
                    var localFiles = Directory.GetFiles(localFolderPath, "*.exe", SearchOption.AllDirectories)
                        .Where(file => candidateNames.Any(name =>
                            string.Equals(Path.GetFileName(file), name, StringComparison.OrdinalIgnoreCase)));
                    candidatePaths.AddRange(localFiles);

                    // Поиск в каталогах из переменной PATH
                    string? pathEnv = Environment.GetEnvironmentVariable("PATH");
                    if (!string.IsNullOrEmpty(pathEnv))
                    {
                        foreach (string dir in pathEnv.Split(Path.PathSeparator))
                        {
                            if (Directory.Exists(dir))
                            {
                                try
                                {
                                    var pathFiles = Directory.GetFiles(dir, "*.exe", SearchOption.TopDirectoryOnly)
                                        .Where(file => candidateNames.Any(name =>
                                            string.Equals(Path.GetFileName(file), name, StringComparison.OrdinalIgnoreCase)));
                                    candidatePaths.AddRange(pathFiles);
                                }
                                catch
                                {
                                    // Игнорируем ошибки доступа
                                }
                            }
                        }
                    }

                    // Если кандидаты с нужными именами не найдены, добавляем значение по умолчанию как fallback.
                    foreach (string candidateName in candidateNames)
                    {
                        if (!candidatePaths.Any(x => string.Equals(Path.GetFileName(x), candidateName, StringComparison.OrdinalIgnoreCase)))
                        {
                            candidatePaths.Add(candidateName);
                        }
                    }

                    // Перебор кандидатов: выбираем первый, для которого зависимость доступна.
                    foreach (string candidate in candidatePaths)
                    {
                        version = DependencyChecker.IsProgramAvailable(candidate, versionArgs, out bool available);
                        if (available)
                        {
                            isAvailable = true;
                            chosenCandidate = candidate;
                            break;
                        }
                    }

                    // Если ни один кандидат не сработал, используем fallback.
                    if (!isAvailable)
                    {
                        chosenCandidate = candidatePaths.FirstOrDefault() ?? defaultExecutable;
                    }
                }

                // Если требуется дополнительная обработка версии, применяем её.
                if (versionProcessor != null && version is not null)
                {
                    version = versionProcessor(version);
                }
            });

            // Возвращаем результат в UI-поток.
            DependencyInfo info = new DependencyInfo
            {
                IsInstalled = isAvailable,
                Path = isAvailable ? chosenCandidate : null,
                Version = version
            };

            return info;
        }

        public async Task LoadYtDlpDependencyAsync()
        {
            var info = await LoadDependencyAsync(
                storedPath: App.AppSettings.Download.YtDlpExePath,
                defaultExecutable: DependenciesConstants.YtDlpExecutable,
                candidateNames: new[] { DependenciesConstants.YtDlpDownloadFilename, DependenciesConstants.YtDlpExecutable },
                versionArgs: DependenciesConstants.YtDlpVersionArgs
            );

            // Обновляем данные для yt-dlp.
            if (info.IsInstalled)
            {
                ytDlpDownloader.YtDlpInfo.Path = info.Path;
                App.AppSettings.Download.YtDlpExePath = info.Path;
            }
            else
            {
                App.AppSettings.Download.YtDlpExePath = null;
                ytDlpDownloader.YtDlpInfo.ResetToNull();
            }
            ytDlpDownloader.YtDlpInfo.Version = info.Version;
            ytDlpDownloader.YtDlpInfo.IsInstalled = info.IsInstalled;
        }

        public async Task LoadFfmpegDependencyAsync()
        {
            var info = await LoadDependencyAsync(
                storedPath: App.AppSettings.Download.FfmpegExePath,
                defaultExecutable: DependenciesConstants.FfmpegExecutable,
                candidateNames: new[] { DependenciesConstants.FfmpegExecutable },
                versionArgs: DependenciesConstants.FfmpegVersionArgs,
                versionProcessor: DependencyChecker.ExtractFfmpegVersion
            );

            // Обновляем данные для ffmpeg.
            if (info.IsInstalled)
            {
                ytDlpDownloader.FfmpegInfo.Path = info.Path;
                App.AppSettings.Download.FfmpegExePath = info.Path;
            }
            else
            {
                App.AppSettings.Download.FfmpegExePath = null;
                ytDlpDownloader.FfmpegInfo.ResetToNull();
            }
            ytDlpDownloader.FfmpegInfo.Version = info.Version;
            ytDlpDownloader.FfmpegInfo.IsInstalled = info.IsInstalled;
        }

    }
}
