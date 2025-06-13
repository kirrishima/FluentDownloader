using FluentDownloader.Helpers;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;

namespace FluentDownloader.Pages
{
    partial class MainPage
    {
        /// <summary>
        /// Displays the tooltip for the download button for a short duration (5 seconds).
        /// </summary>
        private void ShowDownloadButtonToolTip()
        {
            if (_DownloadButtonToolTip is not null)
            {
                _DownloadButtonToolTip.IsOpen = true;

                var timer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromSeconds(5)
                };

                timer.Tick += (s, e) =>
                {
                    _DownloadButtonToolTip.IsOpen = false;
                    timer.Stop();
                };

                timer.Start();
            }
        }

        /// <summary>
        /// Handles the click event of the "Save Path" button. Opens a folder picker dialog
        /// to allow the user to select a folder for saving files.
        /// </summary>
        /// <param name="sender">The source of the event (a Button).</param>
        /// <param name="e">Event data for the click event.</param>
        private async void SavePathButton_Click(object sender, RoutedEventArgs e)
        {
            var senderButton = sender as Button;

            if (senderButton is not null)
            {
                senderButton.IsEnabled = false;
            }

            SavePathTextBox.Text = "";

            FolderPicker openPicker = new FolderPicker();

            IntPtr hWnd = WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow);

            WinRT.Interop.InitializeWithWindow.Initialize(openPicker, hWnd);

            openPicker.SuggestedStartLocation = PickerLocationId.Desktop;
            openPicker.FileTypeFilter.Add("*");

            StorageFolder folder = await openPicker.PickSingleFolderAsync();
            if (folder is not null)
            {
                StorageApplicationPermissions.FutureAccessList.AddOrReplace("PickedFolderToken", folder);
                SavePathTextBox.Text = folder.Path;
                App.AppSettings.General.LastPeekedOutputPath = folder.Path;
            }
            else
            {
                SavePathTextBox.Text = LocalizedStrings.GetMessagesString("SelectSavePathPlaceholder");
            }

            if (senderButton is not null)
            {
                senderButton.IsEnabled = true;
            }
        }

        /// <summary>
        /// Handles the click event to open the URL editing dialog.
        /// Allows the user to edit the URL and saves changes if confirmed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event data for the click event.</param>
        private async void OpenEditUrlDialog_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await dialogSemaphore.WaitAsync();

                EditTextBox.Text = UrlTextBox.Text;

                var result = await EditTextDialog.ShowAsync();

                if (result == ContentDialogResult.Primary)
                {
                    UrlTextBox.Text = EditTextBox.Text;
                }
            }
            finally
            {
                dialogSemaphore.Release();
            }
        }

        /// <summary>
        /// Sets the cursor position at the end of the text in the edit text box when it loads.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event data for the loaded event.</param>
        private void EditTextBox_Loaded(object sender, RoutedEventArgs e)
        {
            EditTextBox.SelectionStart = EditTextBox.Text.Length;
            EditTextBox.SelectionLength = 0;
        }

        /// <summary>
        /// Handles the primary button click event for the edit text dialog.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">Event data for the button click.</param>
        private void EditTextDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // Already handled 
        }

        /// <summary>
        /// Handles the click event of the "Download" button. Checks for dependencies
        /// and triggers the appropriate download or format parsing logic.
        /// </summary>
        /// <param name="sender">The source of the event (a Button).</param>
        /// <param name="e">Event data for the click event.</param>
        private async void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            bool allDepsInstalled = await CheckForDependencies();

            if (!allDepsInstalled)
            {
                return;
            }

            var button = sender as Button;

            if (DownloadButtonProgressRing.Visibility == Visibility.Collapsed
                && DownloadButtonGetFormatsTextBlock.Visibility == Visibility.Visible)
            {
                await ParseVideoFormatsButton_Click(button);
            }
            else if (DownloadButtonCancelPanel.Visibility == Visibility.Visible)
            {
                SetButtonState(button, false);
                DownloadCts?.Cancel();

                SetDownloadButtonState(DownloadButtonState.DownloadVideo);
                SetButtonState(button, true);
            }
            else
            {
                FormatTeachingTip.IsOpen = false;
                await ParseVideoDownloadButton_Click(button);
            }
        }
    }
}