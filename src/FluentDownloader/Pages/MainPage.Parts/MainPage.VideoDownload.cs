using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Linq;

namespace FluentDownloader.Pages
{
    partial class MainPage
    {
        /// <summary>
        /// Enables or disables download-related UI controls
        /// </summary>
        /// <param name="enable">True to enable controls, false to disable them</param>
        private void EnableDownloadOptions(bool enable)
        {
            FormatComboBox.IsEnabled = enable;
            VideoFormatComboBox.IsEnabled = enable;
            AudioFormatComboBox.IsEnabled = enable;
            RecodeVideFormatComboBox.IsEnabled = enable;
            SavePathTextBox.IsEnabled = enable;
            SavePathButton.IsEnabled = enable;
        }

        /// <summary>
        /// Handles successful thumbnail image loading
        /// </summary>
        /// <param name="sender">Image control that triggered the event</param>
        /// <param name="e">ImageOpened event data</param>
        private void ThumbnailImage_OnImageOpened(object sender, RoutedEventArgs e)
        {
            ThumbnailImage.Visibility = Visibility.Visible;
            ThumbnailAnimation.Begin();
        }

        /// <summary>
        /// Handles thumbnail image loading failures
        /// </summary>
        /// <param name="sender">Image control that triggered the event</param>
        /// <param name="e">Exception information from failed load</param>
        /// <remarks>
        /// Sets fallback placeholder image on failure
        /// </remarks>
        private void ThumbnailImage_OnImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            try
            {
                if (sender is Image image)
                {
                    image.Source = new BitmapImage(new Uri("ms-appx:///Assets/thumbnailPlaceholder.png"));
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// Controls visual state of the download button
        /// </summary>
        /// <param name="state">Desired button state to display</param>
        /// <remarks>
        /// Manages visibility of progress ring, text labels and cancel UI
        /// </remarks>
        private void SetDownloadButtonState(DownloadButtonState state)
        {
            ResetDownloadFieldsButton.IsEnabled = state != DownloadButtonState.Processing;

            DownloadButtonProgressRing.Visibility = state == DownloadButtonState.Processing
                ? Visibility.Visible
                : Visibility.Collapsed;

            DownloadButtonDownloadTextBlock.Visibility = state == DownloadButtonState.DownloadVideo
                ? Visibility.Visible
                : Visibility.Collapsed;

            DownloadButtonGetFormatsTextBlock.Visibility = state == DownloadButtonState.ParseFormats
                ? Visibility.Visible
                : Visibility.Collapsed;

            DownloadButtonCancelPanel.Visibility = state == DownloadButtonState.Cancel
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        /// <summary>
        /// Changes enabled state of a button control
        /// </summary>
        /// <param name="button">Target button to modify</param>
        /// <param name="enabled">True to enable, false to disable</param>
        private void SetButtonState(Button? button, bool enabled)
        {
            if (button != null) button.IsEnabled = enabled;
        }

        /// <summary>
        /// Represents possible states of the download button UI
        /// </summary>
        private enum DownloadButtonState
        {
            /// <summary>Format parsing state</summary>
            ParseFormats,

            /// <summary>Video download state</summary>
            DownloadVideo,

            /// <summary>Ongoing processing operation</summary>
            Processing,

            /// <summary>Cancel operation state</summary>
            Cancel
        }

        /// <summary>
        /// Resets all download-related UI elements to default state
        /// </summary>
        /// <param name="sender">Event source (reset button)</param>
        /// <param name="e">Click event data</param>
        /// <remarks>
        /// Clears inputs, resets combo boxes, removes thumbnail and resets progress
        /// </remarks>
        private void ResetDownloadFieldsButton_Click(object sender, RoutedEventArgs e)
        {
            UrlTextBox.Text = string.Empty;
            ResetUrlInputState();

            EnableDownloadOptions(false);

            var items = FormatComboBox.Items.Cast<object>().Take(3).ToArray();
            FormatComboBox.Items.Clear();
            foreach (var item in items)
            {
                FormatComboBox.Items.Add(item);
            }

            _videoData?.ResetToNull();
            ThumbnailImage.Visibility = Visibility.Collapsed;
            ThumbnailImage.DataContext = null;

            SavePathTextBox.IsEnabled = false;

            LogsBlockExpander.IsExpanded = false;
            LogsTextBox.Blocks.Clear();
            UpdateInstallProgress(0);
        }
    }
}