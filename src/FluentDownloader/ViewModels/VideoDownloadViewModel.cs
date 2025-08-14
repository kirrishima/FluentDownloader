using CommunityToolkit.Mvvm.ComponentModel;
using FluentDownloader.Models;

namespace FluentDownloader.ViewModels
{
    public sealed partial class VideoDownloadViewModel : ObservableObject
    {
        private bool _ytdlpServiceIsBusy = false;
        public bool YtdlpServiceIsAvailable
        {
            get => _ytdlpServiceIsBusy;
            set => SetProperty(ref _ytdlpServiceIsBusy, value);
        }

        private bool _isCurrentUrlIsPlaylist;
        public bool IsCurrentUrlIsPlaylist
        {
            get => _isCurrentUrlIsPlaylist;
            set => SetProperty(ref _isCurrentUrlIsPlaylist, value);
        }

        private VideoData _videoData;
        public VideoData VideoData
        {
            get => _videoData;
            set
            {
                OnPropertyChanging(nameof(VideoData));
                _videoData = value;
                OnPropertyChanged(nameof(VideoData));
            }
        }
    }
}
