using System.Diagnostics;
using System.IO;
using System;

namespace FluentDownloader.Helpers.FileSystem
{
    public static class FileSystemHelper
    {
        /// <summary>
        /// Opens the specified file or directory in File Explorer.
        /// </summary>
        /// <param name="filePath">The path to the file or directory to open.</param>
        public static void OpenInFileExplorer(string? filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                return;

            try
            {
                if (File.Exists(filePath))
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = "explorer.exe",
                        Arguments = $"/select,\"{filePath}\"",
                        UseShellExecute = true
                    });
                }
                else if (Directory.Exists(filePath))
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = "explorer.exe",
                        Arguments = $"\"{filePath}\"",
                        UseShellExecute = true
                    });
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to open in File Explorer: {ex.Message}");
            }
        }
    }
}