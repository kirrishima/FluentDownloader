using ABI.System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentDownloader.Models;
using FluentDownloader.Pages;
using FluentDownloader.Services;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.ApplicationModel.Chat;
using Windows.Management.Policies;
using YoutubeDLSharp.Options;

namespace FluentDownloader.ViewModels
{
    public sealed partial class DownloadQueueViewModel : ObservableObject
    {
        private readonly DownloadQueueAnimator _animator;
        private bool _isQueueVisible;
        public ObservableCollection<QueueItem> Items { get; } = [];
        private readonly MainPage MainPage = MainPage.Instance!;

        public int ItemsCount => Items.Count;

        public DownloadQueueViewModel(DownloadQueueAnimator animator)
        {
            _animator = animator;
            _isQueueVisible = false;
            Items.CollectionChanged += Items_CollectionChanged;

            for (int i = 0; i < 10; i++)
            {
                Items.Add(new() { Title = "fdsfds", Status = VideoInQueueStatus.InQueue });
            }
        }

        private void Items_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(ItemsCount));

            MoveUpCommand.NotifyCanExecuteChanged();
            ClearCommand.NotifyCanExecuteChanged();
            ClearSuccededCommand.NotifyCanExecuteChanged();
            MoveDownCommand.NotifyCanExecuteChanged();
            RemoveItemCommand.NotifyCanExecuteChanged();
            AddVideoToQueueCommand.NotifyCanExecuteChanged();
            ToggleQueueCommand.NotifyCanExecuteChanged();
            ResumeCommand.NotifyCanExecuteChanged();
            SkipCommand.NotifyCanExecuteChanged();
            CancelCommand.NotifyCanExecuteChanged();
            RetryFailedCommand.NotifyCanExecuteChanged();
        }

        [RelayCommand(CanExecute = nameof(CanMoveUp))]
        private void MoveUp(QueueItem? item)
        {
            if (item is null) return;
            int idx = Items.IndexOf(item);
            if (idx > 0) Items.Move(idx, idx - 1);
        }

        private bool CanMoveUp(QueueItem? item)
            => item is not null && Items.IndexOf(item) > 0;

        [RelayCommand(CanExecute = nameof(CanMoveDown))]
        private void MoveDown(QueueItem? item)
        {
            if (item is null) return;
            int idx = Items.IndexOf(item);
            if (idx >= 0 && idx < Items.Count - 1) Items.Move(idx, idx + 1);
        }

        private bool CanMoveDown(QueueItem? item)
            => item is not null && Items.IndexOf(item) >= 0 && Items.IndexOf(item) < Items.Count - 1;

        [RelayCommand]
        private void RemoveItem(QueueItem? item)
        {
            if (item is null) return;
            Items.Remove(item);
        }

        private bool CanClear() => Items.Count > 0;

        [RelayCommand(CanExecute = nameof(CanClear))]
        private void Clear()
        {
            Items.Clear();
        }

        private bool CanClearSucceded() => Items.Any(i => i.Status == VideoInQueueStatus.Success);

        [RelayCommand(CanExecute = nameof(CanClearSucceded))]
        private void ClearSucceded()
        {
            foreach (var item in Items.Where(i => i.Status == VideoInQueueStatus.Success).ToList())
            {
                Items.Remove(item);
            }
        }

        [RelayCommand(CanExecute = nameof(CanResume))]
        private void Resume()
        {
            // TODO: Реализация возобновления
        }

        private bool CanResume() => false;

        // Skip
        [RelayCommand(CanExecute = nameof(CanSkip))]
        private void Skip()
        {
            // TODO: Реализация пропуска
        }

        private bool CanSkip() => false;

        // Cancel
        [RelayCommand(CanExecute = nameof(CanCancel))]
        private void Cancel()
        {
            // TODO: Реализация отмены
        }

        private bool CanCancel() => false;

        // RetryFailed
        [RelayCommand(CanExecute = nameof(CanRetryFailed))]
        private void RetryFailed()
        {
            // TODO: Реализация повтора неудачных загрузок
        }

        private bool CanRetryFailed() => false;

        [RelayCommand]
        private async Task ToggleQueueAsync()
        {
            if (_isQueueVisible)
            {
                await _animator.HideQueueAsync();
            }
            else
            {
                await _animator.ShowQueueAsync();
            }

            _isQueueVisible = !_isQueueVisible;
            OnPropertyChanged(nameof(IsQueueVisible));
        }

        public bool IsQueueVisible
        {
            get => _isQueueVisible;
            private set => SetProperty(ref _isQueueVisible, value);
        }

        [RelayCommand]
        private async Task AddVideoToQueueAsync()
        {
            if (MainPage is null || !MainPage.VideoData.HasValue || !MainPage.ValidateSelection() || !MainPage.ValidateDirectoryAccess(out var savePath))
                return;

            var selectedFormat = MainPage.GetSelectedFormat();
            var (mergeFormat, audioFormat, recodeFormat) = MainPage.GetSelectedFormats();
            var defaultFormat = MainPage.GetSelectedDefaultFormat();

            QueueItem item = new()
            {
                Status = VideoInQueueStatus.InQueue,
                Title = MainPage.VideoData.Value.Title,
                IsDefaultFormatSelected = MainPage.IsDefaultFormatSelected(),
                AudioFormat = audioFormat,
                RecodeFormat = recodeFormat,
                MergeFormat = mergeFormat,
                DownloadType = defaultFormat,
                VideoData = MainPage.VideoData.Value,
                VideoFormatInfo = selectedFormat
            };

            Items.Add(item);

            await Task.CompletedTask;
        }

        private bool  CanStartDownload()
        {
            return Items.Any(i => i.Status == VideoInQueueStatus.InQueue);
        }

        [RelayCommand(CanExecute = nameof(CanStartDownload))]
        private async Task StartDownloadAsync()
        {
            while (true)
            {
                MainPage.DownloadCts = new CancellationTokenSource();
                QueueItem? queueItem = Items.FirstOrDefault(i => i.Status == VideoInQueueStatus.InQueue);

                if (queueItem is null || !MainPage.VideoDownloadViewModel.YtdlpServiceIsAvailable)
                {
                    return;
                }

                try
                {
                    MainPage.SetDownloadButtonState(MainPage.DownloadButtonState.Processing);

                    if (!MainPage.ValidateDirectoryAccess(out var savePath))
                        return;

                    MainPage.SetDownloadButtonState(MainPage.DownloadButtonState.Cancel);

                    var mergeFormat = queueItem.MergeFormat;
                    var audioFormat = queueItem.AudioFormat;
                    var recodeFormat = queueItem.RecodeFormat;

                    MainPage.SetProgressBarError(false);
                    MainPage.SetProgressBarPaused(false);
                    MainPage.UpdateInstallProgress(0);

                    queueItem.Status = VideoInQueueStatus.Downloading;

                    bool result;
                    if (queueItem.IsDefaultFormatSelected)
                    {
                        result = await MainPage.HandleDefaultFormatDownload(queueItem.VideoData.Url, savePath, queueItem.DownloadType!.Value, mergeFormat, audioFormat, recodeFormat, MainPage.DownloadCts.Token);
                    }
                    else
                    {
                        result = await MainPage.HandleCustomFormatDownload(queueItem.VideoData.Url, savePath, queueItem.VideoFormatInfo, mergeFormat, audioFormat, recodeFormat, MainPage.DownloadCts.Token);
                    }

                    if (result) MainPage.HandleSuccessfulDownload();
                    queueItem.Status = result ? VideoInQueueStatus.Success : VideoInQueueStatus.Failed;
                }
                catch (System.Exception ex)
                {
                    MainPage.HandleDownloadError(ex);
                    queueItem.Status = VideoInQueueStatus.Failed;
                }
                finally
                {
                    try
                    {
                        MainPage.DownloadCts.Cancel();
                        MainPage.DownloadCts?.Dispose();
                    }
                    catch
                    {

                    }
                }
            }
        }
    }
}
