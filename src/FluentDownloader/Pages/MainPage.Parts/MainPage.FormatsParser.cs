using FluentDownloader.Helpers;
using FluentDownloader.Models;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FluentDownloader.Pages
{
    partial class MainPage
    {
        public VideoData? VideoData { get; private set; }

        /// <summary>
        /// Handles the click event of the ParseVideoFormatsButton.
        /// Trims the URL input, validates it, and attempts to fetch video data.
        /// </summary>
        /// <param name="button">The button that triggered the event. Can be <c>null</c>.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task ParseVideoFormatsButton_Click(Button? button)
        {
            SetButtonState(button, false);
            try
            {
                var url = UrlTextBox.Text.Trim();
                if (!ValidateUrl(url)) return;

                var (success, videoData) = await TryFetchVideoData(url);
                if (!success || !videoData.HasValue)
                {
                    ResetUrlInputState();
                    return;
                }

                ProcessFetchedVideoData(videoData.Value);
            }
            catch (Exception ex)
            {
                HandleGeneralError(ex);
            }
            finally
            {
                SetButtonState(button, true);
            }
        }

        // Подметоды:

        /// <summary>
        /// Validates the specified URL.
        /// </summary>
        /// <param name="url">The URL to validate.</param>
        /// <returns><c>true</c> if the URL is not null or whitespace; otherwise, <c>false</c>.</returns>
        private bool ValidateUrl(string url)
        {
            if (!string.IsNullOrWhiteSpace(url)) return true;

            ShowIncorrectURITeachingTip();

            SetDownloadButtonState(DownloadButtonState.ParseFormats);
            return false;
        }

        void ShowIncorrectURITeachingTip()
        {
            UrlTextBoxTeachingTip.Title = LocalizedStrings.GetMessagesString("InvalidURINotificationTitle");
            UrlTextBoxTeachingTip.Subtitle = LocalizedStrings.GetMessagesString("InvalidURINotificationDescription");
            UrlTextBoxTeachingTip.IsOpen = true;

            var timer = DispatcherQueue.CreateTimer();
            timer.Interval = TimeSpan.FromSeconds(5);
            timer.Tick += (s, e) =>
            {
                UrlTextBoxTeachingTip.IsOpen = false;
                timer.Stop();
            };
            timer.Start();
        }

        /// <summary>
        /// Attempts to fetch video data from the specified URL.
        /// </summary>
        /// <param name="url">The URL from which to fetch video data.</param>
        /// <returns>
        /// A task that returns a tuple where the first item indicates success,
        /// and the second item contains the fetched <see cref="Models.VideoData"/> if successful.
        /// </returns>
        private async Task<(bool success, VideoData? videoData)> TryFetchVideoData(string url)
        {
            try
            {
                SetProcessingState();
                VideoDownloadViewModel.YtdlpServiceIsAvailable = false;
                LogsTextBoxWriteLine(string.Format(LocalizedStrings.GetMessagesString("FetchingVideoDataLogMessage"), url));

                var videoData = await ytDlpDownloader.FetchVideoDataAsync(url);
                VideoData = videoData;

                return (true, videoData);
            }
            catch (Exception ex)
            {
                AddPopUpErrorNotification(ex);
                return (false, null);
            }
            finally
            {
                VideoDownloadViewModel.YtdlpServiceIsAvailable = true;
            }
        }

        /// <summary>
        /// Sets the UI to a processing state during video data fetching.
        /// </summary>
        private void SetProcessingState()
        {
            SetDownloadButtonState(DownloadButtonState.Processing);
            UrlTextBox.IsEnabled = false;
            OpenEditUrlButton.IsEnabled = false;
        }

        /// <summary>
        /// Processes the fetched video data by checking for errors and updating the UI accordingly.
        /// </summary>
        /// <param name="videoData">The fetched video data.</param>
        private void ProcessFetchedVideoData(VideoData videoData)
        {
            if (videoData.VideoFormats.Count == 0 && !videoData.IsPlaylist)
            {
                var errorText = LocalizedStrings.GetMessagesString("FetchingVideoDataErorLogMessage");
                LogsTextBoxWriteLine(errorText);
                HandleVideoDataErrors();
                ResetUrlInputState();
                return;
            }

            PopulateFormatComboBox(videoData);
            EnableDownloadOptions(true);
            SetSuccessState();
        }

        /// <summary>
        /// Populates the format combo box with video format options from the fetched video data.
        /// </summary>
        /// <param name="videoData">The fetched video data.</param>
        private void PopulateFormatComboBox(VideoData videoData)
        {
            var preservedItems = FormatComboBox.Items.Take(3).ToList();

            var formats = videoData.VideoFormats
                .SelectMany((resolution, index) => resolution.Value
                    .Select(format => new ComboBoxItem
                    {
                        Content = format.ToString(),
                        Tag = index
                    }))
                .ToList();

            FormatComboBox.Items.Clear();
            preservedItems.ForEach(item => FormatComboBox.Items.Add(item));
            formats.ForEach(item => FormatComboBox.Items.Add(item));

            LogsTextBoxWriteLine(LocalizedStrings.GetMessagesString("FetchingVideoDataSuccessLogMessage"));
        }

        /// <summary>
        /// Sets the UI state to indicate a successful video data fetch and binds the thumbnail image.
        /// </summary>
        private void SetSuccessState()
        {
            SetDownloadButtonState(DownloadButtonState.DownloadVideo);
            ThumbnailImage.DataContext = VideoData;
        }

        /// <summary>
        /// Handles errors in the fetched video data by displaying a notification with error details.
        /// </summary>
        private void HandleVideoDataErrors()
        {
            var errorDetails = VideoData.HasValue && VideoData.Value.Errors.Any()
                ? string.Join(Environment.NewLine, VideoData.Value.Errors)
                : "Null";
            var description = string.Format(LocalizedStrings.GetMessagesString("FormatRetrievalErrorDescription"), errorDetails);

            AddPopUpNotification(
                LocalizedStrings.GetMessagesString("FormatRetrievalErrorTitle"),
                description,
                InfoBarSeverity.Error);
            ResetUrlInputState();
        }

        /// <summary>
        /// Resets the URL input state to allow the user to try again.
        /// </summary>
        private void ResetUrlInputState()
        {
            UrlTextBox.IsEnabled = true;
            OpenEditUrlButton.IsEnabled = true;
            SetDownloadButtonState(DownloadButtonState.ParseFormats);
            SetProgressBarError(false);
            SetProgressBarPaused(false);
        }

        /// <summary>
        /// Handles a general error by resetting the download button state and displaying an error notification.
        /// </summary>
        /// <param name="ex">The exception that occurred.</param>
        private void HandleGeneralError(Exception ex)
        {
            ResetUrlInputState();
            AddPopUpErrorNotification(ex);
        }
    }
}
