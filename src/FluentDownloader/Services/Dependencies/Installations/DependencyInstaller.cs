using FluentDownloader.Models;
using FluentDownloader.Services.Dependencies.Helpers;
using System;
using System.IO;
using System.Threading.Tasks;

namespace FluentDownloader.Services.Dependencies.Installations
{
    /// <summary>
    /// Provides functionality for installing program dependencies by downloading required files 
    /// or archives and managing environment variables.
    /// </summary>
    public static class DependencyInstaller
    {
        /// <summary>
        /// Asynchronously installs a program dependency by downloading the required file or archive
        /// and optionally adding it to the system's PATH environment variable.
        /// </summary>
        /// <param name="programName">The name of the program to install (e.g., "ffmpeg").</param>
        /// <param name="versionArg">The argument used to check the program's version (e.g., "--version").</param>
        /// <param name="downloadUrl">The URL from which the program or archive will be downloaded.</param>
        /// <param name="destinationPath">The local path where the downloaded file or extracted contents will be stored.</param>
        /// <param name="isArchive">
        /// Indicates whether the downloaded file is a ZIP archive that needs extraction. Default is <c>false</c>.
        /// </param>
        /// <param name="promptUser">
        /// A delegate that prompts the user for confirmation, such as adding the program to PATH.
        /// Returns <c>true</c> if the user agrees, or <c>false</c> otherwise. Default is <c>null</c>.
        /// </param>
        /// <param name="logMessage">
        /// A delegate for logging messages during the installation process.
        /// Accepts a string message as input. Default is <c>null</c>.
        /// </param>
        /// <param name="printProgress">An optional action to report the download progress in percentage.</param>
        /// <returns>
        /// A task that returns <c>true</c> if the program was successfully installed; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>
        /// This method follows these steps:
        /// <list type="number">
        /// <item>Checks if the program is already installed by verifying its availability and version.</item>
        /// <item>Downloads the file or archive from the specified URL.</item>
        /// <item>If the file is an archive, extracts its contents and searches for the target executable.</item>
        /// <item>Optionally prompts the user to add the program's directory to the system's PATH.</item>
        /// </list>
        /// If any step fails, an appropriate message is logged, and the method returns <c>false</c>.
        /// </remarks>
        public static async Task<bool> InstallDependencyAsync(
            string programName,
            string versionArg,
            string downloadUrl,
            string destinationPath,
            bool isArchive = false,
            Func<string, string, string, Task<bool>>? promptUser = null,
            Action<string?>? logMessage = null,
            Action<int>? printProgress = null)
        {
            logMessage?.Invoke($"Checking if {programName} is already installed...");
            string version = DependencyChecker.IsProgramAvailable(programName, versionArg, out bool isInstalled) ?? "Unknown";

            if (isInstalled && programName == DependenciesConstants.FfmpegExecutable)
            {
                version = DependencyChecker.ExtractFfmpegVersion(version);
            }

            if (isInstalled)
            {
                logMessage?.Invoke($"{programName} already installed. Version: {version}");
                return true;
            }

            logMessage?.Invoke($"{programName} is not installed.");
            logMessage?.Invoke($"Downloading {programName}...");

            var downloadResult = isArchive
                ? await DependencyDownloader.DownloadAndExtractZipAsync(downloadUrl, destinationPath, DependenciesConstants.FfmpegExecutable, printProgress)
                : await DependencyDownloader.DownloadFileAsync(downloadUrl, destinationPath, printProgress);

            if (downloadResult.DownloadedPath != null && downloadResult.Exception == null)
            {
                logMessage?.Invoke($"{programName} downloaded successfully.");
                return true;
            }

            if (downloadResult.Exception != null)
            {
                logMessage?.Invoke($"Error downloading {programName}: {downloadResult.Exception.Message}");
                return false;
            }

            return false;
        }
    }
}
