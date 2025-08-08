using FluentDownloader.Models;
using FluentDownloader.Services;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FluentDownloader.ViewModels
{
    public class DownloadQueueViewModel : ViewModelBase
    {
        private readonly DownloadQueueAnimator _animator;
        private bool _isQueueVisible;
        public ObservableCollection<QueueItem> Items { get; } = [];

        public DownloadQueueViewModel(DownloadQueueAnimator animator)
        {
            _animator = animator;
            // Изначально очередь скрыта
            _isQueueVisible = false;
            ToggleQueueCommand = new RelayCommand(async () => await ToggleQueueAsync());

            for (int i = 0; i < 10; i++)
            {
                Items.Add(new QueueItem { Title = "Лучшее видео + лучшее аудио", Size = "1243,5MB", Status = "В очереди" });
                Items.Add(new QueueItem { Title = "Еще один файл", Size = "512MB", Status = "В очереди" });
            }
        }

        /// <summary>
        /// Команда для переключения видимости очереди.
        /// </summary>
        public ICommand ToggleQueueCommand { get; }

        /// <summary>
        /// Выполняет анимацию: показывает или скрывает очередь.
        /// </summary>
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
    }
}
