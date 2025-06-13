using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using System;
using FluentDownloader.Helpers;
using Microsoft.UI.Text;
using CommunityToolkit.WinUI;
using System.Threading.Tasks;

namespace FluentDownloader.Pages
{
    public partial class MainPage
    {
        const int MaxNotifications = 5;

        /// <summary>
        /// Checks if the given text exceeds the specified maximum width when rendered with the given font size.
        /// </summary>
        /// <param name="text">The text to check.</param>
        /// <param name="maxWidth">The maximum allowed width for the text.</param>
        /// <param name="fontSize">The font size to use for the measurement.</param>
        /// <returns>True if the text is ellipsized (exceeds the width), otherwise false.</returns>
        private bool IsTextEllipsizedAsync(string text, double maxWidth, double fontSize)
        {
            char[] chars = new char[3] { '\n', '\r', '\t' };
            if (text.IndexOfAny(chars) != -1)
            {
                return true;
            }

            var textBlock = new TextBlock
            {
                Text = text,
                FontSize = fontSize,
                TextWrapping = TextWrapping.NoWrap,
                TextTrimming = TextTrimming.None
            };

            textBlock.Measure(new Windows.Foundation.Size(double.PositiveInfinity, double.PositiveInfinity));
            return textBlock.DesiredSize.Width > maxWidth;
        }


        /// <summary>
        /// Adds a pop-up error notification with a predefined title and the specified message.
        /// </summary>
        /// <param name="message">The error message to display.</param>
        public void AddPopUpErrorNotification(string message)
        {
            AddPopUpNotification(
                LocalizedStrings.GetMessagesString("PopUpErrorNotificationString"),
                message,
                InfoBarSeverity.Error);
        }

        /// <summary>
        /// Adds a pop-up error notification for an exception, displaying its type, message, and stack trace.
        /// </summary>
        /// <param name="exception">The exception to display.</param>
        public void AddPopUpErrorNotification(Exception exception)
        {
            AddPopUpNotification(
                string.Format(LocalizedStrings.GetMessagesString("PopUpErrorNotificationException"), exception.GetType().FullName),
                $"{exception.Message}\n{exception.StackTrace}",
                InfoBarSeverity.Error);
        }

        double NotificationStackWidth = -1;

        /// <summary>
        /// Adds a customizable pop-up notification with a title, message, and severity level.
        /// </summary>
        /// <param name="title">The title of the notification.</param>
        /// <param name="message">The message content of the notification.</param>
        /// <param name="severity">The severity level of the notification (e.g., Error, Warning).</param>
        public async void AddPopUpNotification(string title, string message, InfoBarSeverity severity)
        {
            await DispatcherQueue.EnqueueAsync(() =>
            {
                double maxContentWidth = NotificationStack.ActualWidth;

                //NotificationStackWidth != NotificationStack.ActualWidth
                //&& NotificationStack.ActualWidth != 0
                //? NotificationStack.ActualWidth
                //: NotificationStackWidth;

                const double fontSize = 14;

                message = message.Trim();
                bool isEllipsized = IsTextEllipsizedAsync(message, maxContentWidth, fontSize);

                var textBlock = new TextBlock
                {
                    Text = message,
                    TextTrimming = TextTrimming.CharacterEllipsis,
                    TextWrapping = TextWrapping.NoWrap,
                    MaxLines = 1,
                    VerticalAlignment = VerticalAlignment.Center,
                    FontSize = fontSize,
                    MaxWidth = maxContentWidth
                };

                var contentGrid = new Grid
                {
                    ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
                },
                    Padding = new Thickness(0, 0, 0, 10),
                };

                contentGrid.Children.Add(textBlock);

                if (isEllipsized)
                {
                    contentGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

                    var showMoreButton = new Button
                    {
                        Content = LocalizedStrings.GetMessagesString("PopUpNotificationShowMore"),
                        HorizontalAlignment = HorizontalAlignment.Right,
                        VerticalAlignment = VerticalAlignment.Center,
                        Margin = new Thickness(10, 0, 0, 0)
                    };

                    showMoreButton.Click += (s, e) =>
                    {
                        ShowFullNotification(title, message);
                    };

                    contentGrid.Children.Add(showMoreButton);
                    Grid.SetColumn(showMoreButton, 1);
                }

                var infoBar = new InfoBar
                {
                    Title = title,
                    Severity = severity,
                    IsOpen = true,
                    Opacity = 0,
                    Content = contentGrid
                };

                textBlock.Loaded += (sender, args) => { textBlock.MaxWidth = infoBar.ActualWidth; };

                var fadeInAnimation = new DoubleAnimation
                {
                    From = 0,
                    To = 1,
                    Duration = TimeSpan.FromMilliseconds(300)
                };

                var fadeInStoryboard = new Storyboard();
                Storyboard.SetTarget(fadeInAnimation, infoBar);
                Storyboard.SetTargetProperty(fadeInAnimation, "Opacity");
                fadeInStoryboard.Children.Add(fadeInAnimation);

                fadeInStoryboard.Begin();

                infoBar.CloseButtonClick += (s, e) =>
                {
                    FadeOutAndShiftNotifications(infoBar);
                };

                NotificationStack.Children.Insert(0, infoBar);

                if (NotificationStack.Children.Count > MaxNotifications)
                {
                    var hiddenNotification = NotificationStack.Children[MaxNotifications] as InfoBar;
                    if (hiddenNotification != null)
                    {
                        hiddenNotification.Visibility = Visibility.Collapsed;
                    }
                }
            });
        }

        /// <summary>
        /// Displays a full notification dialog with the complete message content.
        /// </summary>
        /// <param name="message">The full message to display.</param>
        private async void ShowFullNotification(string title, string message)
        {
            var titleBlock = new TextBlock()
            {
                Text = title,
                TextWrapping = TextWrapping.Wrap,
                MaxWidth = 600,
                FontWeight = FontWeights.Bold
            };
            var dialog = new ContentDialog
            {
                Title = titleBlock /*LocalizedStrings.GetMessagesString("PopUpNotificationTitle")*/,
                Content = new ScrollViewer
                {
                    Content = new TextBlock
                    {
                        Text = message,
                        TextWrapping = TextWrapping.Wrap,
                        MaxWidth = 600
                    }
                },
                CloseButtonText = LocalizedStrings.GetMessagesString("PopUpNotificationCloseButton"),
                XamlRoot = this.Content.XamlRoot,

            };

            await dialog.ShowAsync();
        }

        /// <summary>
        /// Fades out the specified notification and shifts the remaining notifications up.
        /// </summary>
        /// <param name="infoBar">The notification to fade out and remove.</param>
        private void FadeOutAndShiftNotifications(InfoBar infoBar)
        {
            var fadeOutAnimation = new DoubleAnimation
            {
                From = 1,
                To = 0,
                Duration = TimeSpan.FromMilliseconds(300)
            };

            var fadeOutStoryboard = new Storyboard();
            Storyboard.SetTarget(fadeOutAnimation, infoBar);
            Storyboard.SetTargetProperty(fadeOutAnimation, "Opacity");

            fadeOutStoryboard.Completed += (s, e) =>
            {
                if (NotificationStack.Children.Contains(infoBar))
                {
                    NotificationStack.Children.Remove(infoBar);
                }

                for (int i = 0; i < NotificationStack.Children.Count; i++)
                {
                    var hiddenNotification = NotificationStack.Children[MaxNotifications - 1] as InfoBar;
                    if (hiddenNotification != null)
                    {
                        hiddenNotification.Visibility = Visibility.Visible;
                    }
                }
            };

            fadeOutStoryboard.Begin();
        }
    }
}
