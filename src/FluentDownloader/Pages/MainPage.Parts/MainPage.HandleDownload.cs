using FluentDownloader.Helpers;
using FluentDownloader.Helpers.FileSystem;
using FluentDownloader.Models;
using FluentDownloader.Services.Ytdlp.Helpers;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using YoutubeDLSharp.Options;

namespace FluentDownloader.Pages
{
    partial class MainPage
    {
        /// <summary>
        /// Handles the click event for the video download button.
        /// Validates the selected format and directory access, processes the download request, and handles errors.
        /// </summary>
        /// <param name="button">The button that triggered the event. Can be null.</param>
        private async Task ParseVideoDownloadButton_Click(Button? button)
        {
            DownloadCts = new CancellationTokenSource();
            try
            {
                SetDownloadButtonState(DownloadButtonState.Processing);

                if (!ValidateSelection() || !ValidateDirectoryAccess(out var savePath))
                    return;

                SetDownloadButtonState(DownloadButtonState.Cancel);

                var (mergeFormat, audioFormat, recodeFormat) = GetSelectedFormats();

                SetProgressBarError(false);
                SetProgressBarPaused(false);
                UpdateInstallProgress(0);

                await ProcessDownloadRequest(savePath, mergeFormat, audioFormat, recodeFormat, DownloadCts.Token);

                HandleSuccessfulDownload();
            }
            catch (Exception ex)
            {
                HandleDownloadError(ex);
            }
            finally
            {
                DownloadCts.Cancel();
                DownloadCts?.Dispose();
            }
        }

        #region Helper Methods

        /// <summary>
        /// Validates whether a format has been selected in the format combo box.
        /// If no format is selected, it shows a teaching tip and resets the UI state.
        /// </summary>
        /// <returns>True if a format is selected; otherwise, false.</returns>
        public bool ValidateSelection()
        {
            if (FormatComboBox.SelectedIndex >= 0) return true;

            DispatcherQueue.TryEnqueue(async () =>
            {
                FormatTeachingTip.StartBringIntoView();
                await Task.Delay(100);
                FormatTeachingTip.IsOpen = true;
            });

            ResetToInitialState();
            return false;
        }

        /// <summary>
        /// Validates whether the selected directory is accessible for saving files.
        /// If the directory is not accessible, it shows an error and resets the UI state.
        /// </summary>
        /// <param name="savePath">The path to the directory where files will be saved.</param>
        /// <returns>True if the directory is accessible; otherwise, false.</returns>
        public bool ValidateDirectoryAccess(out string savePath)
        {
            savePath = SavePathTextBox.Text;
            var directoryStatus = DirectoryAccessChecker.CheckDirectoryAccess(savePath);

            if (directoryStatus.IsAccessible)
            {
                App.AppSettings.General.LastPeekedOutputPath = savePath;
                return true;
            }

            ShowDirectoryError(directoryStatus);
            ResetToInitialState();
            return false;
        }

        /// <summary>
        /// Retrieves the selected formats for video, audio, and recoding from the respective combo boxes.
        /// </summary>
        /// <returns>A tuple containing the selected merge format, audio format, and recode format.</returns>
        public (DownloadMergeFormat mergeFormat, AudioConversionFormat audioFormat, VideoRecodeFormat recodeFormat) GetSelectedFormats()
        {
            return (
                GetSelectedFormat<DownloadMergeFormat>(VideoFormatComboBox, DownloadMergeFormat.Unspecified),
                GetSelectedFormat<AudioConversionFormat>(AudioFormatComboBox, AudioConversionFormat.Best),
                GetSelectedFormat<VideoRecodeFormat>(RecodeVideFormatComboBox, VideoRecodeFormat.None)
            );
        }

        /// <summary>
        /// Retrieves the selected format from a combo box.
        /// If no format is selected, it returns the default value.
        /// </summary>
        /// <typeparam name="T">The type of the enum representing the format.</typeparam>
        /// <param name="comboBox">The combo box containing the format options.</param>
        /// <param name="defaultValue">The default value to return if no format is selected.</param>
        /// <returns>The selected format or the default value.</returns>
        private T GetSelectedFormat<T>(ComboBox comboBox, T defaultValue) where T : Enum
        {
            return comboBox.SelectedItem is ComboBoxItem { Tag: int tag }
                ? (T)(object)tag
                : defaultValue;
        }

        /// <summary>
        /// Processes the download request based on the selected format.
        /// Handles both default and custom format selections.
        /// </summary>
        /// <param name="savePath">The path where the downloaded file will be saved.</param>
        /// <param name="mergeFormat">The selected merge format for the video and audio.</param>
        /// <param name="audioFormat">The selected audio format.</param>
        /// <param name="recodeFormat">The selected video recode format.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task ProcessDownloadRequest(string savePath, DownloadMergeFormat mergeFormat,
            AudioConversionFormat audioFormat, VideoRecodeFormat recodeFormat, CancellationToken cancellationToken)
        {
            bool result = false;
            if (IsDefaultFormatSelected())
            {
                result = await HandleDefaultFormatDownload(savePath, mergeFormat, audioFormat, recodeFormat, cancellationToken);
            }
            else
            {
                result = await HandleCustomFormatDownload(savePath, mergeFormat, audioFormat, recodeFormat, cancellationToken);
            }
            if (!result)
            {
                await Task.Delay(1000); // wait for yt-dlp process to release files
                FragmentCleaner.CleanUpFragments(savePath, LogsTextBoxWriteLine);
            }
        }

        /// <summary>
        /// Checks if the default format (Best Video, Best Audio, or Merged) is selected.
        /// </summary>
        /// <returns>True if a default format is selected; otherwise, false.</returns>  
        private bool IsDefaultFormatSelected() => FormatComboBox.SelectedIndex < PreservedItemsCount;

        /// <summary>
        /// Handles the download process for default formats (Best Video, Best Audio, or Merged).
        /// </summary>
        /// <param name="savePath">The path where the downloaded file will be saved.</param>
        /// <param name="mergeFormat">The selected merge format for the video and audio.</param>
        /// <param name="audioFormat">The selected audio format.</param>
        /// <param name="recodeFormat">The selected video recode format.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task<bool> HandleDefaultFormatDownload(string savePath, DownloadMergeFormat mergeFormat,
            AudioConversionFormat audioFormat, VideoRecodeFormat recodeFormat, CancellationToken cancellationToken)
        {
            var downloadType = (DownloadType)FormatComboBox.SelectedIndex;
            var res = await ytDlpDownloader.DownloadVideo(
                 null,
                UrlTextBox.Text,
                downloadPath: savePath,
                mergeFormat: mergeFormat,
                audioFormat: audioFormat,
                onlyaudio: downloadType == DownloadType.BestAudio,
                onlyvideo: downloadType == DownloadType.BestVideo,
                bv_ba: downloadType == DownloadType.Merged,
                recodeFormat: recodeFormat,
                cancellationToken: cancellationToken,
                videoData: VideoData
            );

            UpdateInstallProgress(100);
            return res;
        }

        public VideoFormatInfo? GetSelectedFormat()
        {
            if (FormatComboBox.SelectedItem is not ComboBoxItem { Tag: int index })
            {
                var downloadType = (DownloadType)FormatComboBox.SelectedIndex;

                var (mergeFormat, audioFormat, recodeFormat) = GetSelectedFormats();
                string? res = null;

                switch (downloadType)
                {
                    case DownloadType.BestVideo:
                        res = LocalizedStrings.GetResourceString("FormatComboBoxItem1/Content");
                        break;
                    case DownloadType.BestAudio:
                        res = LocalizedStrings.GetResourceString("FormatComboBoxItem2/Content");
                        break;
                    case DownloadType.Merged:
                        res = LocalizedStrings.GetResourceString("FormatComboBoxItem3/Content");
                        break;
                    default:
                        break;
                }

                return new VideoFormatInfo(resolution: res!, extension: mergeFormat.ToString(), null, null, null!, true, null);
            }

            var selectedFormat = VideoData?.VideoFormats
                .SelectMany(r => r.Value)
                .ElementAtOrDefault(index);

            return selectedFormat;
        }

        /// <summary>
        /// Handles the download process for custom formats.
        /// </summary>
        /// <param name="savePath">The path where the downloaded file will be saved.</param>
        /// <param name="mergeFormat">The selected merge format for the video and audio.</param>
        /// <param name="audioFormat">The selected audio format.</param>
        /// <param name="recodeFormat">The selected video recode format.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task<bool> HandleCustomFormatDownload(string savePath, DownloadMergeFormat mergeFormat,
            AudioConversionFormat audioFormat, VideoRecodeFormat recodeFormat, CancellationToken cancellationToken)
        {
            var selectedFormat = GetSelectedFormat();

            if (selectedFormat == null)
            {
                return false;
            }

            var res = await ytDlpDownloader.DownloadVideo(
                selectedFormat,
                UrlTextBox.Text,
                savePath,
                mergeFormat,
                audioFormat,
                recodeFormat: recodeFormat,
                cancellationToken: cancellationToken,
                videoData: VideoData
            );

            UpdateInstallProgress(100);

            return res;
        }

        /// <summary>
        /// Handles the UI updates and logging after a successful download.
        /// </summary>
        private void HandleSuccessfulDownload()
        {
            LogsTextBoxWriteLine();
            SetDownloadButtonState(DownloadButtonState.DownloadVideo);
        }

        /// <summary>
        /// Displays an error message related to directory access issues.
        /// </summary>
        /// <param name="status">The status object containing the error details.</param>
        private void ShowDirectoryError(DirectoryStatus status)
        {
            SavePathTeachingTip.Title = status.Title;
            SavePathTeachingTip.Subtitle = status.Subtitle;
            DispatcherQueue.TryEnqueue(async () =>
            {
                SavePathTeachingTip.StartBringIntoView();
                await Task.Delay(100);
                SavePathTeachingTip.IsOpen = true;
            });
        }

        /// <summary>
        /// Resets the UI to its initial state, enabling controls and clearing selections.
        /// </summary>
        private void ResetToInitialState()
        {
            SetProgressBarError(false);
            SetProgressBarPaused(false);
            UpdateInstallProgress(0);
            SetDownloadButtonState(DownloadButtonState.DownloadVideo);
            SetButtonState(button: null, true);
        }

        /// <summary>
        /// Handles errors that occur during the download process.
        /// Displays an error notification and resets the UI state.
        /// </summary>
        /// <param name="ex">The exception that occurred during the download process.</param>
        private void HandleDownloadError(Exception ex)
        {
            SetDownloadButtonState(DownloadButtonState.DownloadVideo);
            AddPopUpErrorNotification(ex);
        }

        #endregion

        #region Supporting Types
        /// <summary>
        /// Represents user selected donload type in <see cref="FormatComboBox"/>
        /// </summary>
        private enum DownloadType
        {
            BestVideo = 0,
            BestAudio = 1,
            Merged = 2
        }

        /// <summary>
        /// Default formats, eg <see cref="DownloadType"/>
        /// </summary>
        private const int PreservedItemsCount = 3;
        #endregion
    }
}
