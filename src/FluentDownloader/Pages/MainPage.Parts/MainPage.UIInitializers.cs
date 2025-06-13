using FluentDownloader.Extensions;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Diagnostics;
using System.Linq;
using Windows.Foundation;
using YoutubeDLSharp.Options;

namespace FluentDownloader.Pages
{
    partial class MainPage
    {
        /// <summary>
        /// Initializes the download options UI elements and adjusts their layout dynamically.
        /// </summary>
        private void InitializeDownloadOptions()
        {
            VideoFormatComboBox.PopulateComboBoxWithEnum(DownloadMergeFormat.Mp4);
            AudioFormatComboBox.PopulateComboBoxWithEnum(AudioConversionFormat.Best);
            RecodeVideFormatComboBox.PopulateComboBoxWithEnum(VideoRecodeFormat.None);

            AdjustComboBoxWidth(VideoFormatComboBox);
            AdjustComboBoxWidth(AudioFormatComboBox);
            AdjustComboBoxWidth(RecodeVideFormatComboBox);
            AdjustComboBoxWidth(FormatComboBox);

            // This code implements behavior similar to CSS flex wrapping.  
            // If the screen is not wide enough, combo boxes will align into two lines instead of one.  
            // Can be replaced with CommunityToolkit.WinUI.Controls.WrapPanel but is considered more "efficient."

            bool isStacked = false;
            const double hysteresis = 10;

            var Adjust = () =>
            {
                try
                {
                    double horizontalRequiredWidth =
                        VideoFormatComboBox.ActualWidth +
                        VideoFormatComboBox.Padding.Left + VideoFormatComboBox.Padding.Right +

                        AudioFormatComboBox.ActualWidth +
                        AudioFormatComboBox.Padding.Left + AudioFormatComboBox.Padding.Right +

                        RecodeVideFormatComboBox.ActualWidth +
                        RecodeVideFormatComboBox.Padding.Left + RecodeVideFormatComboBox.Padding.Right +
                        FormatComboBox.ActualWidth + FormatComboBox.Padding.Left + FormatComboBox.Padding.Right;

                    if (!isStacked && FormatGrid.ActualWidth < horizontalRequiredWidth - hysteresis)
                    {
                        RelativePanel.SetBelow(AudioFormatComboBox, FormatComboBox);
                        RelativePanel.SetRightOf(AudioFormatComboBox, null);
                        AudioFormatComboBox.Margin = new Thickness(0, 10, 0, 0);
                        RecodeVideFormatComboBox.Margin = new Thickness(10, 10, 0, 0);
                        isStacked = true;
                    }
                    else if (isStacked && FormatGrid.ActualWidth > horizontalRequiredWidth + hysteresis)
                    {
                        RelativePanel.SetBelow(AudioFormatComboBox, null);
                        RelativePanel.SetRightOf(AudioFormatComboBox, VideoFormatComboBox);
                        AudioFormatComboBox.Margin = new Thickness(10, 0, 0, 0);
                        RecodeVideFormatComboBox.Margin = new Thickness(10, 0, 0, 0);
                        isStacked = false;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"{ex.GetType().FullName}: {ex.Message}\n{ex.InnerException} {ex.StackTrace}");
                }
            };

            FormatGrid.Loaded += (s, e) => Adjust();
            FormatGrid.SizeChanged += (s, e) =>
            {
                DispatcherQueue.TryEnqueue(() => Adjust());
            };
        }

        /// <summary>
        /// Adjusts the width of the specified combo box based on the widest item and header.
        /// </summary>
        /// <param name="comboBox">The combo box whose width is to be adjusted.</param>
        private void AdjustComboBoxWidth(ComboBox comboBox)
        {
            var size = new Size(double.PositiveInfinity, double.PositiveInfinity);
            double maxItemWidth = comboBox.Items.Select(i => new TextBlock() { Text = (string)((ComboBoxItem)i).Content })
                .Max(textBlock => { textBlock.Measure(size); return textBlock.DesiredSize.Width; });

            var tex = new TextBlock() { Text = ((string)comboBox.Header) };
            tex.Measure(size);
            double headerWidth = tex.DesiredSize.Width;
            if (headerWidth > maxItemWidth + comboBox.Padding.Left + 30)
            {
                comboBox.Width = headerWidth;
            }
            else
            {
                comboBox.Width = Math.Max(maxItemWidth, headerWidth) + comboBox.Padding.Left + 38;
            }
        }
    }
}
