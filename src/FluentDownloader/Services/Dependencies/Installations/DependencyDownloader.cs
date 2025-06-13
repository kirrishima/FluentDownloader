using System;
using System.IO.Compression;
using System.Threading.Tasks;
using System.Net.Http;
using System.IO;
using System.Linq;
using System.Diagnostics;
using FluentDownloader.Services.Dependencies.Exceptions;

namespace FluentDownloader.Services.Dependencies.Installations
{
    /// <summary>
    /// Provides utility methods for downloading files and handling ZIP archives.
    /// </summary>
    /// <remarks>
    /// This class enables downloading files from a given URL, optionally tracking progress, 
    /// and extracting ZIP archives while managing errors gracefully.
    /// </remarks>
    public static class DependencyDownloader
    {
        /// <summary>
        /// Asynchronously downloads a file from a given URL and saves it to a specified destination.
        /// </summary>
        /// <param name="url">The URL of the file to be downloaded.</param>
        /// <param name="destinationPath">The local path where the file will be saved.</param>
        /// <param name="printProgress">An optional action to report the download progress in percentage.</param>
        /// <returns>
        /// A tuple containing:
        /// <list type="bullet">
        /// <item><c>DownloadedPath</c>: The full path of the downloaded file if successful; otherwise, <c>null</c>.</item>
        /// <item><c>Exception</c>: An exception if an error occurs; otherwise, <c>null</c>.</item>
        /// </list>
        /// </returns>
        /// <remarks>
        /// - Uses <see cref="HttpClient"/> for downloading.
        /// - Provides progress updates if <paramref name="printProgress"/> is supplied.
        /// - Returns an exception if the request fails.
        /// </remarks>
        public static async Task<(string? DownloadedPath, Exception? Exception)> DownloadFileAsync(
            string url, string destinationPath, Action<int>? printProgress)
        {
            try
            {
                using (var httpClient = new HttpClient())
                using (var response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        return (null, new DependencyDownloaderException($"Failed to download from {url}: {response.StatusCode} - {response.ReasonPhrase}"));
                    }

                    var contentLength = response.Content.Headers.ContentLength;

                    if (contentLength == null)
                    {
                        return (null, new DependencyDownloaderException("Content length is not available."));
                    }

                    using (var contentStream = await response.Content.ReadAsStreamAsync())
                    using (var fileStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true))
                    {
                        var buffer = new byte[8192];
                        long totalBytesRead = 0;
                        int bytesRead;
                        int prevPercentage = -1;

                        while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                        {
                            await fileStream.WriteAsync(buffer, 0, bytesRead);
                            totalBytesRead += bytesRead;

                            var progressPercentage = (int)((double)totalBytesRead / contentLength.Value * 100);
                            if (progressPercentage > prevPercentage)
                            {
                                printProgress?.Invoke(progressPercentage);
                                prevPercentage = progressPercentage;
                            }
                        }
                    }

                    return (destinationPath, null);
                }
            }
            catch (Exception ex)
            {
                return (null, new DependencyDownloaderException($"Error while downloading from {url}: " +
                    $"{ex.Message}.\nStack Trace:\n{ex.StackTrace}"));
            }
        }

        /// <summary>
        /// Downloads a ZIP archive from a given URL, extracts its contents, and deletes the archive afterward.
        /// </summary>
        /// <param name="url">The URL of the ZIP archive.</param>
        /// <param name="extractPath">The directory where the archive contents should be extracted.</param>
        /// <param name="destinationExecutable">The target file to locate within the extracted files.</param>
        /// <param name="printProgress">An optional action to report the download progress.</param>
        /// <returns>
        /// A tuple containing:
        /// <list type="bullet">
        /// <item><c>DownloadedPath</c>: The full path to the target executable if found; otherwise, <c>null</c>.</item>
        /// <item><c>Exception</c>: An exception if an error occurs; otherwise, <c>null</c>.</item>
        /// </list>
        /// </returns>
        /// <remarks>
        /// This method follows these steps:
        /// <list type="number">
        /// <item>Downloads the ZIP archive via <see cref="DownloadFileAsync"/>.</item>
        /// <item>Extracts the archive to the specified location.</item>
        /// <item>Searches for the specified executable or file within the extracted directory.</item>
        /// <item>Deletes the ZIP archive upon successful extraction.</item>
        /// </list>
        /// If any step fails, it returns a descriptive exception.
        /// </remarks>
        public static async Task<(string? DownloadedPath, Exception? Exception)> DownloadAndExtractZipAsync(
            string url, string extractPath, string destinationExecutable, Action<int>? printProgress)
        {
            string zipPath = Path.Combine(extractPath, "temp.zip");
            try
            {
                var downloadResult = await DownloadFileAsync(url, zipPath, printProgress);

                if (downloadResult.Exception != null)
                {
                    return downloadResult;
                }

                ZipFile.ExtractToDirectory(zipPath, extractPath);
                File.Delete(zipPath);
                return (Directory.GetFiles(extractPath, destinationExecutable, SearchOption.AllDirectories).FirstOrDefault(), null);
            }
            catch (Exception ex)
            {
                return (null, new Exceptions.DependencyDownloaderException($"Error while unpacking \"{zipPath}\" to" +
                    $" \"{extractPath}\": {ex.Message}.\nStack Trace:\n{ex.StackTrace}"));
            }
        }
    }
}
