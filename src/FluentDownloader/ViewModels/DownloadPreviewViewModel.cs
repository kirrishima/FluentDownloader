using CommunityToolkit.Mvvm.ComponentModel;

namespace FluentDownloader.ViewModels
{
    public sealed partial class DownloadPreviewViewModel : ObservableObject
    {
        private string? _thumbnailUri = null!;
        public string? ThumbnailUri
        {
            get => _thumbnailUri;
            set => SetProperty(ref _thumbnailUri, value);
        }

        private string? _title = null!;
        public string? Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public void SetData(string? thumbnailUri, string? title)
        {
            ThumbnailUri = thumbnailUri;
            Title = title;
        }

        public void ResetData()
        {
            SetData(null!, null!);
        }
    }
}
