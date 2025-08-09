using CommunityToolkit.Mvvm.ComponentModel;

namespace FluentDownloader.ViewModels
{
    public sealed partial class VideoDownloadViewModel : ObservableObject
    {
        private bool _ytdlpServiceIsBusy = false;
        public bool YtdlpServiceIsBusy
        {
            get => _ytdlpServiceIsBusy;
            set => SetProperty(ref _ytdlpServiceIsBusy, value);
        }
    }
}
