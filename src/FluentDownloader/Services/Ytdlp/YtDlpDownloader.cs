using FluentDownloader.Dialogs;
using FluentDownloader.Helpers;
using FluentDownloader.Models;
using FluentDownloader.Services.Dependencies.Models;
using FluentDownloader.Services.Ytdlp.Helpers;
using Microsoft.UI.Xaml.Controls;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using YoutubeDLSharp;
using YoutubeDLSharp.Options;

namespace FluentDownloader.Services.Ytdlp
{
    /// <summary>
    /// Class for downloading videos using yt-dlp.
    /// </summary>
    public class YtDlpDownloader
    {
        /// <summary>
        /// Information about the yt-dlp dependency.
        /// </summary>
        public DependencyInfo YtDlpInfo { get; set; }

        /// <summary>
        /// Information about the ffmpeg dependency.
        /// </summary>
        public DependencyInfo FfmpegInfo { get; set; }

        YoutubeDL youtubeDl = null!;

        private readonly IDialogService _dialogService;
        private readonly IDownloadDependencies _downloadDependencies;
        private readonly IProgressBar _progressBar;

        public YtDlpDownloader(
            IDialogService dialogService,
            IDownloadDependencies downloadDependencies,
            IProgressBar progressBar)
        {
            _dialogService = dialogService;
            _downloadDependencies = downloadDependencies;
            _progressBar = progressBar;

            YtDlpInfo = new DependencyInfo();
            FfmpegInfo = new DependencyInfo();
        }

        /// <summary>
        /// Creates download options for the video.
        /// </summary>
        /// <param name="videoFormatInfo">Video format information.</param>
        /// <param name="mergeFormat">Merge format.</param>
        /// <param name="recodeFormat">Recode format (optional).</param>
        /// <param name="onlyaudio">Flag to download only audio.</param>
        /// <param name="onlyvideo">Flag to download only video.</param>
        /// <param name="bv_ba">Flag to download best video and best audio.</param>
        /// <param name="audioFormat">Audio conversion format.</param>
        /// <param name="concurrentFragments">Number of concurrent download fragments.</param>
        /// <returns>An <see cref="OptionSet"/> object with download parameters.</returns>
        public static OptionSet CreateDownloadOptions(
            VideoFormatInfo? videoFormatInfo,
            DownloadMergeFormat? mergeFormat,
            VideoRecodeFormat? recodeFormat = null,
            bool onlyaudio = false,
            bool onlyvideo = false,
            bool bv_ba = false,
            AudioConversionFormat? audioFormat = null
            )
        {
            var options = new OptionSet
            {
                ConcurrentFragments = App.AppSettings.Download.YtdlpConcurrentFragments,
                MergeOutputFormat = mergeFormat ?? DownloadMergeFormat.Unspecified,
                AudioFormat = audioFormat ?? AudioConversionFormat.M4a,
                EmbedThumbnail = true,
                RecodeVideo = recodeFormat ?? VideoRecodeFormat.None,
                Encoding = "utf8",
            };

            if (App.AppSettings.Download.UseRateLimit && App.AppSettings.Download.RateLimitInBytes > 0)
            {
                options.LimitRate = App.AppSettings.Download.RateLimitInBytes;
            }

            if (App.AppSettings.Download.UseProxy && !string.IsNullOrWhiteSpace(App.AppSettings.Download.Proxy))
            {
                options.Proxy = App.AppSettings.Download.Proxy;
            }

            options.AddCustomOption("--encoding", "utf-8");
            if (onlyaudio)
            {
                options.ExtractAudio = true;
                options.Format = "bestaudio";
            }
            else if (onlyvideo)
            {
                options.Format = "bestvideo";
            }
            else if (bv_ba)
            {
                options.Format = "bestvideo+bestaudio/best";
            }
            else
            {
                if (videoFormatInfo is not null)
                {
                    if (videoFormatInfo.Value.HasAudio)
                    {
                        options.Format = videoFormatInfo.Value.FormatId;
                    }
                    else
                    {
                        options.Format = $"{videoFormatInfo.Value.FormatId}+bestaudio";
                    }
                }
            }

            return options;
        }

        /// <summary>
        /// Ensures the YoutubeDL instance is initialized.
        /// </summary>
        /// <returns>Returns true if the instance is created.</returns>
        private bool EnsureYoutubeDL()
        {
            if (youtubeDl is null)
            {
                youtubeDl = new YoutubeDL()
                {
                    YoutubeDLPath = YtDlpInfo.Path,
                    FFmpegPath = FfmpegInfo.Path
                };
            }
            return youtubeDl != null;
        }

        /// <summary>
        /// Fetches video data from the given URL.
        /// </summary>
        /// <param name="url">Video URL.</param>
        /// <returns>A <see cref="VideoData"/> object containing video information.</returns>
        public async Task<VideoData?> FetchVideoDataAsync(string url)
        {
            if (!EnsureYoutubeDL())
            {
                return null;
            }

            var videoInfo = await youtubeDl.RunVideoDataFetch(url);

            if (videoInfo.Success && videoInfo.Data != null &&
                videoInfo.Data.Entries != null && videoInfo.Data.Entries.Any())
            {
                var entry = videoInfo.Data.Entries.First();
                var data = (await FetchVideoDataAsync(entry.Url)).Value;

                return new VideoData(
                    [],
                    data.ThumbnailUri,
                    "Playlist",
                    videoInfo.Data.ID,
                    videoInfo.ErrorOutput)
                { IsPlaylist = true };
            }

            if (videoInfo.Success)
            {
                var formats = videoInfo.Data?.Formats
                        .Where(f => f.VideoCodec != null && f.Height.HasValue)
                        .GroupBy(f => f.Height ?? -1)
                        .OrderByDescending(g => g.Key)
                        .ToDictionary(
                            g => g.Key,
                            g => g.OrderByDescending(f => f.Bitrate ?? 0)
                                  .ThenByDescending(f => f.FrameRate ?? 0)
                                  .Select(f => new VideoFormatInfo(f.Resolution, f.Extension, f.FrameRate, f.Height, f.FormatId,
                                                                        !string.IsNullOrEmpty(f.AudioCodec) && f.AudioCodec != "none"))
                                  .DistinctBy(i => i.Extension)
                                  .ToList()
                        );

                if (formats != null && videoInfo.Data is not null)
                {
                    return new VideoData(formats, videoInfo.Data.Thumbnail, videoInfo.Data.Title, videoInfo.Data.ID, videoInfo.ErrorOutput);
                }
            }
            return new VideoData(videoInfo.ErrorOutput);
        }

        /// <summary>
        /// Downloads a video from the given URL with specified parameters.
        /// </summary>
        /// <param name="videoFormatInfo">Video format information.</param>
        /// <param name="videoUrl">Video URL.</param>
        /// <param name="downloadPath">Path to save the downloaded file.</param>
        /// <param name="mergeFormat">Merge format.</param>
        /// <param name="audioFormat">Audio conversion format.</param>
        /// <param name="recodeFormat">Recode format.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <param name="onlyaudio">Flag to download only audio.</param>
        /// <param name="onlyvideo">Flag to download only video.</param>
        /// <param name="bv_ba">Flag to download best video and best audio.</param>
        /// <returns>True if the download is successful, otherwise false.</returns>
        public async Task<bool> DownloadVideo(
            VideoFormatInfo? videoFormatInfo,
            string videoUrl,
            string downloadPath,
            DownloadMergeFormat mergeFormat,
            AudioConversionFormat audioFormat,
            VideoRecodeFormat recodeFormat,
            CancellationToken cancellationToken,
            bool onlyaudio = false,
            bool onlyvideo = false,
            bool bv_ba = false, VideoData? videoData = null)
        {
            if (EnsureYoutubeDL())
            {
                OptionSet optionSet;

                if (videoFormatInfo is null)
                {
                    if (!onlyaudio && !onlyvideo && !bv_ba)
                    {
                        await _dialogService.ShowNotificationDialogAsync(
                            string.Format(LocalizedStrings.GetMessagesString("UnknowErrorInDonwloadVideo"), nameof(DownloadVideo)),
                            LocalizedStrings.GetMessagesString("UnknowErrorInDonwloadVideoDescription")
                            );
                        return false;
                    }
                }

                optionSet = CreateDownloadOptions(
                    videoFormatInfo,
                    mergeFormat,
                    audioFormat: audioFormat,
                    recodeFormat: recodeFormat,
                    onlyvideo: onlyvideo,
                    onlyaudio: onlyaudio,
                    bv_ba: bv_ba);

                optionSet.Output = Path.Combine(App.AppSettings.General.LastPeekedOutputPath, App.AppSettings.Download.FileOutputTemplate);
                optionSet.Verbose = false;
                optionSet.Progress = true;
                optionSet.NoProgress = false;

                optionSet.YesPlaylist = videoData.HasValue && videoData.Value.IsPlaylist;

                var progress = new LogBoxProgress(_dialogService, _downloadDependencies, _progressBar);

                youtubeDl.OutputFolder = downloadPath;

                _progressBar.UpdateInstallProgress(0);
                try
                {
                    var downloadResult = await youtubeDl.RunVideoDownload(
                        videoUrl,
                        output: progress,
                        overrideOptions: optionSet,
                        ct: cancellationToken
                    );

                    _progressBar.UpdateInstallProgress(100);
                    if (downloadResult.Success)
                    {
                        _dialogService.AddPopUpNotification(
                             LocalizedStrings.GetMessagesString("FileDownloadedSuccessfully"),
                             string.Format(LocalizedStrings.GetMessagesString("FileSavedAs"), downloadResult.Data),
                             InfoBarSeverity.Success
                             );

                        if (App.AppSettings.Notifications.EnableDesktopNotifications)
                        {
                            NotificationService.ShowNotificationWithImage(LocalizedStrings.GetMessagesString("FileDownloadedSuccessfully"),
                               string.Format(LocalizedStrings.GetMessagesString("FileSavedAs"), downloadResult.Data),
                               videoData?.ThumbnailUri);
                        }
                    }
                    else
                    {
                        if (App.AppSettings.Notifications.EnableDesktopNotifications)
                        {
                            NotificationService.ShowNotificationWithImage(
                             string.Format(string.Format(LocalizedStrings.GetMessagesString("ErrorWhileDownloading"), videoUrl)),
                             string.Join(Environment.NewLine, downloadResult.ErrorOutput),
                             videoData?.ThumbnailUri);
                        }

                        _dialogService.AddPopUpNotification(
                            string.Format(string.Format(LocalizedStrings.GetMessagesString("ErrorWhileDownloading"), videoUrl)),
                            string.Join(Environment.NewLine, downloadResult.ErrorOutput), InfoBarSeverity.Error
                            );
                    }
                    return downloadResult.Success;
                }
                catch (OperationCanceledException)
                {
                    _dialogService.AddPopUpNotification(
                        LocalizedStrings.GetMessagesString("OperationCancelled"),
                        string.Format(LocalizedStrings.GetMessagesString("OperationCancelledForUrlFormat"), videoUrl),
                        InfoBarSeverity.Informational
                        );
                    _downloadDependencies.LogsTextBoxWriteLine(LocalizedStrings.GetMessagesString("OperationCancelled"));
                    _progressBar.SetProgressBarPaused(true);
                }
                catch (Exception ex)
                {
                    _dialogService.AddPopUpNotification(
                    string.Format(string.Format(LocalizedStrings.GetMessagesString("ErrorWhileDownloading"), videoUrl)),
                    string.Join(Environment.NewLine, ex.Message), InfoBarSeverity.Error
                    );
                    _progressBar.SetProgressBarError(true);
                }
            }
            return false;
        }
    }
}