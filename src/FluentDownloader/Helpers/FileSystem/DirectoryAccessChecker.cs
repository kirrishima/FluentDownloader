using System;
using System.IO;

namespace FluentDownloader.Helpers.FileSystem
{
    /// <summary>
    /// Предоставляет методы для проверки доступа к директории.
    /// Provides methods for checking directory access permissions.
    /// </summary>
    public static class DirectoryAccessChecker
    {
        /// <summary>
        /// Checks whether the specified directory is accessible.
        /// </summary>
        /// <param name="path">The directory path to check.</param>
        /// <returns>Объект <see cref="DirectoryStatus"/>, содержащий информацию о статусе директории.</returns>
        public static DirectoryStatus CheckDirectoryAccess(string path)
        {
            var status = new DirectoryStatus
            {
                Title = LocalizedStrings.GetMessagesString("DirectoryAccessCheckerBaseTitle"),
                Subtitle = LocalizedStrings.GetMessagesString("DirectoryAccessCheckerBaseSubtitle"),
                IsAccessible = false
            };

            if (string.IsNullOrWhiteSpace(path) || path == LocalizedStrings.GetMessagesString("SelectSavePathPlaceholder"))
            {
                status.Title = LocalizedStrings.GetMessagesString("DirectoryAccessCheckerNullOrWhiteSpaceTitle");
                status.Subtitle = LocalizedStrings.GetMessagesString("DirectoryAccessCheckerNullOrWhiteSpaceSubtitle");
                return status;
            }

            try
            {
                if (Directory.Exists(path))
                {
                    var files = Directory.GetFiles(path);
                    var directories = Directory.GetDirectories(path);

                    //string randomFilePath;
                    //do
                    //{
                    //    randomFilePath = Path.Combine(path, Guid.NewGuid().ToString() + ".tmp");
                    //}
                    //while (File.Exists(randomFilePath));

                    //File.Create(randomFilePath);
                    //File.Decrypt(randomFilePath);

                    status.Title = LocalizedStrings.GetMessagesString("DirectoryAccessCheckerOK");
                    status.Subtitle = LocalizedStrings.GetMessagesString("DirectoryAccessCheckerAccessible");
                    status.IsAccessible = true;
                }
                else
                {
                    status.Title = LocalizedStrings.GetMessagesString("DirectoryAccessCheckerNotFoundTitle");
                    status.Subtitle = LocalizedStrings.GetMessagesString("DirectoryAccessCheckerNotFoundSubtitle");
                }
            }
            catch (UnauthorizedAccessException)
            {
                status.Title = LocalizedStrings.GetMessagesString("DirectoryAccessCheckerAccessDeniedTitle");
                status.Subtitle = LocalizedStrings.GetMessagesString("DirectoryAccessCheckerAccessDeniedSubtitle");
            }
            catch (IOException ex)
            {
                status.Title = LocalizedStrings.GetMessagesString("DirectoryAccessCheckerIOTitle");
                status.Subtitle = string.Format(LocalizedStrings.GetMessagesString("DirectoryAccessCheckerIOSubtitle"), ex.Message);
            }
            catch (Exception ex)
            {
                status.Title = LocalizedStrings.GetMessagesString("DirectoryAccessCheckerErrorTitle");
                status.Subtitle = string.Format(LocalizedStrings.GetMessagesString("DirectoryAccessCheckerErrorSubtitle"), ex.Message);
            }

            return status;
        }
    }

    /// <summary>
    /// Представляет статус проверки доступа к директории.
    /// Represents the status of a directory access check.
    /// </summary>
    public class DirectoryStatus
    {
        public string? Title { get; set; } = null;
        public string? Subtitle { get; set; } = null;
        public bool IsAccessible { get; set; }
    }
}
