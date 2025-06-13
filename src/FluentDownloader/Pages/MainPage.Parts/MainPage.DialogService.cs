using CommunityToolkit.WinUI;
using FluentDownloader.Helpers;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using System;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FluentDownloader.Pages
{
    public partial class MainPage
    {
        /// <summary>
        /// Displays a notification dialog with the specified title and text content.
        /// </summary>
        /// <param name="title">The title of the notification dialog. Can be <c>null</c>.</param>
        /// <param name="content">The textual content to display in the notification.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task ShowNotificationDialogAsync(string? title, string content)
        {
            await DispatcherQueue.EnqueueAsync(async () =>
             {
                 var Content = new TextBlock
                 {
                     Text = content,
                     TextWrapping = TextWrapping.Wrap,
                     FontSize = 16
                 };
                 await ShowNotificationDialogAsync(title, Content);
             });
        }

        /// <summary>
        /// Displays a notification dialog with the specified title and content.
        /// </summary>
        /// <param name="title">The title of the notification dialog. Can be <c>null</c>.</param>
        /// <param name="content">The content to display in the notification dialog. This can be any object that is supported by the dialog.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task ShowNotificationDialogAsync(string? title, object content)
        {
            await DispatcherQueue.EnqueueAsync(async () =>
              {
                  try
                  {
                      await dialogSemaphore.WaitAsync();

                      ContentDialog notificationDialog = new ContentDialog
                      {
                          //Background = StylesManager.ContentDialogBackgroundBrush,
                          XamlRoot = this.Content.XamlRoot,
                          Title = title,
                          RequestedTheme = RootThemeElement.RequestedTheme,
                          Content = content,
                          CloseButtonText = GetDialogsString("NotificationDialogCloseButton"),
                          DefaultButton = ContentDialogButton.Close
                      };

                      await notificationDialog.ShowAsync();
                  }
                  finally
                  {
                      dialogSemaphore.Release();
                  }
              });
        }

        /// <summary>
        /// Displays an installation dialog with primary and close buttons.
        /// </summary>
        /// <param name="title">The title of the installation dialog. Can be <c>null</c>.</param>
        /// <param name="content">The content to display in the dialog. This can be any object that is supported by the dialog.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result is <c>true</c> if the primary button was pressed; otherwise, <c>false</c>.
        /// </returns>
        public async Task<bool> ShowInstallDialogAsync(string? title, object content)
        {
            return await DispatcherQueue.EnqueueAsync(async () =>
            {
                try
                {
                    await dialogSemaphore.WaitAsync();

                    ContentDialog dialog = new ContentDialog
                    {
                        XamlRoot = this.Content.XamlRoot,
                        Title = title,
                        RequestedTheme = RootThemeElement.RequestedTheme,
                        Content = content,
                        PrimaryButtonText = GetDialogsString("NotificationDialogPrimaryButtonText"),
                        CloseButtonText = GetDialogsString("NotificationDialogCloseButtonText"),
                        DefaultButton = ContentDialogButton.Primary
                    };

                    var result = await dialog.ShowAsync();
                    return result == ContentDialogResult.Primary;
                }
                finally
                {
                    dialogSemaphore.Release();
                }
            });
        }

        /// <summary>
        /// Prompts the user with a dialog containing a message and two options, and returns a boolean indicating the user's choice.
        /// </summary>
        /// <param name="title">The title of the prompt dialog. Can be <c>null</c>.</param>
        /// <param name="content">The text content of the prompt message.</param>
        /// <param name="acceptOption">The text for the accept (primary) button.</param>
        /// <param name="rejectOption">The text for the reject (close) button.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result is <c>true</c> if the user selects the accept option; otherwise, <c>false</c>.
        /// </returns>
        public async Task<bool> PromptUserAsync(string? title, string content, string acceptOption, string rejectOption)
        {
            return await DispatcherQueue.EnqueueAsync(async () =>
            {
                try
                {
                    await dialogSemaphore.WaitAsync();

                    ContentDialog dialog = new ContentDialog
                    {
                        XamlRoot = this.Content.XamlRoot,
                        Title = title,
                        RequestedTheme = RootThemeElement.RequestedTheme,
                        Content = new TextBlock
                        {
                            Text = content,
                            TextWrapping = TextWrapping.Wrap
                        },
                        PrimaryButtonText = acceptOption,
                        CloseButtonText = rejectOption,
                        DefaultButton = ContentDialogButton.Primary
                    };

                    var result = await dialog.ShowAsync();

                    return result == ContentDialogResult.Primary;
                }
                finally
                {
                    dialogSemaphore.Release();
                }
            });
        }

        /// <summary>
        /// Appends a new line of text to the logs text box.
        /// </summary>
        /// <param name="text">The text to write. If <c>null</c>, an empty line is added.</param>
        public async void LogsTextBoxWriteLine(string? text = null)
        {
            await DispatcherQueue.EnqueueAsync(() =>
            {
                var paragraph = new Paragraph();
                paragraph.Inlines.Add(new Run { Text = text });
                LogsTextBox.Blocks.Add(paragraph);
                LogsTextBox.UpdateLayout();
                LogsScrollViewer.ChangeView(null, LogsScrollViewer.ExtentHeight, null);
            });
        }

        /// <summary>
        /// Overrides the last line in the logs text box with the specified text.
        /// </summary>
        /// <param name="text">The text to set on the last line. If <c>null</c>, the line is replaced with an empty line.</param>
        public async void LogsTextBoxOverrideLine(string? text = null)
        {
            await DispatcherQueue.EnqueueAsync(() =>
            {
                var paragraph = new Paragraph();
                paragraph.Inlines.Add(new Run { Text = text });
                LogsTextBox.Blocks.RemoveAt(LogsTextBox.Blocks.Count - 1);
                LogsTextBox.Blocks.Add(paragraph);
                LogsTextBox.UpdateLayout();
                LogsScrollViewer.ChangeView(null, LogsScrollViewer.ExtentHeight, null);
            });
        }

        /// <summary>
        /// Updates the progress of the installation by setting the value of the progress bar.
        /// </summary>
        /// <param name="progress">An integer representing the current progress percentage.</param>
        public async void UpdateInstallProgress(int progress)
        {
            await DispatcherQueue.EnqueueAsync(() => { DownloadingProgressBar.Value = progress; });
        }

        public async void SetProgressBarPaused(bool paused)
        {
            await DispatcherQueue.EnqueueAsync(() => { ProgressBar.ShowPaused = paused; });
        }

        public async void SetProgressBarError(bool error)
        {
            await DispatcherQueue.EnqueueAsync(() => { ProgressBar.ShowError = error; });
        }

        public async void SetProgressBarRunning(bool running)
        {
            await DispatcherQueue.EnqueueAsync(() => { ProgressBar.IsIndeterminate = running; });
        }
    }
}
