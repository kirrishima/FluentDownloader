using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentDownloader.Models;
using FluentDownloader.Pages;
using FluentDownloader.Services;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Management.Policies;
using YoutubeDLSharp.Options;

namespace FluentDownloader.ViewModels
{
    public sealed partial class DownloadQueueViewModel : ObservableObject
    {
        private readonly DownloadQueueAnimator _animator;
        private bool _isQueueVisible;
        public ObservableCollection<QueueItem> Items { get; } = [];

        public int ItemsCount => Items.Count;

        public DownloadQueueViewModel(DownloadQueueAnimator animator)
        {
            _animator = animator;
            // Изначально очередь скрыта
            _isQueueVisible = false;
            //ToggleQueueCommand = new RelayCommand(async () => await ToggleQueueAsync());

            for (int i = 0; i < 0; i++)
            {
                Items.Add(new QueueItem { Title = "Лучшее видео + лучшее аудио", Size = "1243,5MB", Status = "В очереди" });
                Items.Add(new QueueItem { Title = "Еще один файл", Size = "512MB", Status = "В очереди" });
            }

            Items.CollectionChanged += Items_CollectionChanged;
        }

        private void Items_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            // Обновляем биндинг на количество
            OnPropertyChanged(nameof(ItemsCount));

            // Сообщаем генерации команд пересчитать CanExecute
            // сгенерированные свойства: MoveUpCommand, MoveDownCommand, RemoveItemCommand, AddVideoToQueueCommand и т.д.
            // Все они реализуют IRelayCommand / IAsyncRelayCommand и имеют NotifyCanExecuteChanged()
            MoveUpCommand.NotifyCanExecuteChanged();
            MoveDownCommand.NotifyCanExecuteChanged();
            RemoveItemCommand.NotifyCanExecuteChanged();
            AddVideoToQueueCommand.NotifyCanExecuteChanged();
            ToggleQueueCommand.NotifyCanExecuteChanged();
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

        // MoveDown
        [RelayCommand(CanExecute = nameof(CanMoveDown))]
        private void MoveDown(QueueItem? item)
        {
            if (item is null) return;
            int idx = Items.IndexOf(item);
            if (idx >= 0 && idx < Items.Count - 1) Items.Move(idx, idx + 1);
        }

        private bool CanMoveDown(QueueItem? item)
            => item is not null && Items.IndexOf(item) >= 0 && Items.IndexOf(item) < Items.Count - 1;

        // Remove
        [RelayCommand]
        private void RemoveItem(QueueItem? item)
        {
            if (item is null) return;
            Items.Remove(item);
        }

        /// <summary>
        /// Команда для переключения видимости очереди.
        /// </summary>
        //public ICommand ToggleQueueCommand { get; }

        /// <summary>
        /// Выполняет анимацию: показывает или скрывает очередь.
        /// </summary>
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
            // Если надо уведомить UI об изменении состояния
            OnPropertyChanged(nameof(IsQueueVisible));
        }

        /// <summary>
        /// Признак видимости очереди (можно привязать для UI-логики например).
        /// </summary>
        public bool IsQueueVisible
        {
            get => _isQueueVisible;
            private set => SetProperty(ref _isQueueVisible, value);
        }

        [RelayCommand]
        private async Task AddVideoToQueueAsync()
        {
            var mainPage = MainPage.Instance;

            if (mainPage is null || !mainPage.VideoData.HasValue || !mainPage.ValidateSelection() || !mainPage.ValidateDirectoryAccess(out var savePath))
                return;

            //var (mergeFormat, audioFormat, recodeFormat) = mainPage.GetSelectedFormats();
            var selectedFormat = mainPage.GetSelectedFormat();

            QueueItem item = new()
            {
                Status = "В очереди",
                Title = mainPage.VideoData.Value.Title,
                Resolution = $"{selectedFormat?.Resolution} {selectedFormat?.Extension}",
                Size = selectedFormat?.FileSize?.ToString() ?? string.Empty
            };

            Items.Add(item);

            await Task.CompletedTask;
        }
    }
}
