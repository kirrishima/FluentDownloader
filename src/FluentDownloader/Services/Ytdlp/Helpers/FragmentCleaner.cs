using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace FluentDownloader.Services.Ytdlp.Helpers
{
    /// <summary>
    /// Provides functionality to clean up fragment files created during interrupted downloads.
    /// </summary>
    public static class FragmentCleaner
    {
        // Regex pattern to match extensions ending with -Frag[number]
        private static readonly Regex FragmentPattern = new(@"\w+-Frag\d+$", RegexOptions.Compiled);

        /// <summary>
        /// Deletes files whose extension ends with -Frag[number] pattern.
        /// </summary>
        /// <param name="directoryPath">Path to the directory to clean.</param>
        /// <param name="log">Logging delegate for status messages.</param>
        public static void CleanUpFragments(string directoryPath, Action<string> log)
        {
            try
            {
                if (!Directory.Exists(directoryPath))
                {
                    log($"[FragmentCleaner] Directory not found: {directoryPath}");
                    return;
                }

                // Get files where the last segment after '.' matches -Frag[number] pattern
                var filesToDelete = Directory.GetFiles(directoryPath)
                    .Where(f =>
                    {
                        var fileName = Path.GetFileName(f);
                        var lastSegment = fileName.Split('.')[^1];  // Get part after last '.', must be extension
                        return FragmentPattern.IsMatch(lastSegment);
                    })
                    .ToList();

                foreach (var file in filesToDelete)
                {
                    try
                    {
                        File.Delete(file);
                        log($"[FragmentCleaner] Deleted fragment: {Path.GetFileName(file)}");
                    }
                    catch (Exception ex)
                    {
                        log($"[FragmentCleaner] Failed to delete {file}: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                log($"[FragmentCleaner] Critical error during cleanup: {ex.Message}");
            }
        }
    }
}