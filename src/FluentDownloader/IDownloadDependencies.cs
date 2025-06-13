using FluentDownloader.Models;
using Microsoft.UI.Xaml.Controls;

namespace FluentDownloader
{
    /// <summary>
    /// Defines methods for interaction between helper classes for installing dependencies and window/page
    /// </summary>
    public interface IDownloadDependencies
    {
        /// <summary>
        /// Logs <paramref name="text"/> to logs text box and appends newline at the end.
        /// </summary>
        /// <param name="text"></param>
        void LogsTextBoxWriteLine(string? text = null);

        /// <summary>
        /// Logs <paramref name="text"/> to text box overriding previous line
        /// </summary>
        /// <param name="text"></param>
        public void LogsTextBoxOverrideLine(string? text = null);
    }
}
