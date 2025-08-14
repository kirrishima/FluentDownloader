using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeDLSharp.Options;
using static FluentDownloader.Pages.MainPage;

namespace FluentDownloader.Models
{
    public partial class QueueItem : ObservableObject
    {
        public DownloadMergeFormat MergeFormat { get; set; }
        public VideoRecodeFormat RecodeFormat { get; set; }
        public AudioConversionFormat AudioFormat { get; set; }
        public VideoFormatInfo? VideoFormatInfo { get; set; }
        public PlaylistDownloadSettings? PlaylistDownloadSettings { get; set; }
        public DownloadType? DownloadType { get; set; }
        public bool IsDefaultFormatSelected { get; set; }
        public string Title { get; set; } = null!;
        public string Size => VideoFormatInfo?.FileSize?.ToString() ?? string.Empty;
        private VideoInQueueStatus _status;
        public VideoInQueueStatus Status
        {
            get => _status;
            set
            {
                SetProperty(ref _status, value);
                OnPropertyChanged(nameof(StatusDisplay));
                OnPropertyChanged(nameof(BadgeBackground));
                OnPropertyChanged(nameof(BadgeForeground));
            }
        }

        public string StatusDisplay => Status.GetLocalizedDisplayName();

        public Brush BadgeForeground
        {
            get
            {
                switch (Status)
                {
                    case VideoInQueueStatus.Success:
                        return SystemSignalColors.SystemFillColorSuccessBrush;
                    case VideoInQueueStatus.InQueue:
                        return SystemSignalColors.SystemFillColorAttentionBrush;
                    case VideoInQueueStatus.Failed:
                        return SystemSignalColors.SystemFillColorCriticalBrush;
                    case VideoInQueueStatus.Downloading:
                        return SystemSignalColors.SystemFillColorCautionBrush;
                    default:
                        return new SolidColorBrush(Colors.Transparent);
                }
            }
        }

        public Brush BadgeBackground
        {
            get
            {
                switch (Status)
                {
                    case VideoInQueueStatus.Success:
                        return SystemSignalColors.SystemFillColorSuccessBackgroundBrush;
                    case VideoInQueueStatus.InQueue:
                        return SystemSignalColors.SystemFillColorAttentionBackgroundBrush;
                    case VideoInQueueStatus.Failed:
                        return SystemSignalColors.SystemFillColorCriticalBackgroundBrush;
                    case VideoInQueueStatus.Downloading:
                        return SystemSignalColors.SystemFillColorCautionBackgroundBrush;
                    default:
                        return new SolidColorBrush(Colors.Transparent);
                }
            }
        }

        public string Resolution => $"{VideoFormatInfo?.Resolution} {VideoFormatInfo?.Extension}";

        public VideoData VideoData { get; set; }
    }

    public static class SystemSignalColors
    {
        private static Brush ResolveBrush(string resourceKey)
        {
            var app = App.Current;
            if (app?.Resources != null &&
                app.Resources.TryGetValue(resourceKey, out var obj) &&
                obj is Brush brush)
            {
                return brush;
            }

            // fallback — не возвращаем null
            return new SolidColorBrush(Colors.Transparent);
        }

        // Success
        public static Brush SystemFillColorSuccessBrush =>
            ResolveBrush("SystemFillColorSuccessBrush");

        public static Brush SystemFillColorSuccessBackgroundBrush =>
            ResolveBrush("SystemFillColorSuccessBackgroundBrush");

        // Caution
        public static Brush SystemFillColorCautionBrush =>
            ResolveBrush("SystemFillColorCautionBrush");

        public static Brush SystemFillColorCautionBackgroundBrush =>
            ResolveBrush("SystemFillColorCautionBackgroundBrush");

        // Critical
        public static Brush SystemFillColorCriticalBrush =>
            ResolveBrush("SystemFillColorCriticalBrush");

        public static Brush SystemFillColorCriticalBackgroundBrush =>
            ResolveBrush("SystemFillColorCriticalBackgroundBrush");

        // Attention
        public static Brush SystemFillColorAttentionBrush =>
            ResolveBrush("SystemFillColorAttentionBrush");

        public static Brush SystemFillColorAttentionBackgroundBrush =>
            ResolveBrush("SystemFillColorAttentionBackgroundBrush");
    }
}
