using System;
using System.ComponentModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace FluentDownloader.ViewModels
{
    // playlist range vm: использует observableobject; команда реализована вручную, чтобы избежать зависимостей генератора
    public partial class PlaylistRangeViewModel : ObservableObject
    {
        // вход: длина плейлиста
        private int _playlistLength;
        public int PlaylistLength
        {
            get => _playlistLength;
            set
            {
                if (SetProperty(ref _playlistLength, value))
                {
                    // вызываем обновление только если значение действительно изменилось
                    UpdatePlaylistInfoText();
                    ValidateAll();
                }
            }
        }

        // база индексов: true -> 0-based, false -> 1-based
        private bool _isZeroBasedIndex = false;
        public bool IsZeroBasedIndex
        {
            get => _isZeroBasedIndex;
            set
            {
                if (SetProperty(ref _isZeroBasedIndex, value))
                {
                    ValidateAll();
                }
            }
        }

        // конструктор (подписываемся на изменения download vm)
        public PlaylistRangeViewModel(VideoDownloadViewModel? downloadViewModel = null)
        {
            if (downloadViewModel != null)
            {
                downloadViewModel.PropertyChanged += DownloadViewModel_PropertyChanged;
            }

            //ApplyRangeCommand = new SimpleRelayCommand(OnApply, CanApply);
            UpdatePlaylistInfoText();
        }

        private void DownloadViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(VideoDownloadViewModel.VideoData))
            {
                // если sender не VideoDownloadViewModel — защитимся
                if (sender is VideoDownloadViewModel vm && vm.VideoData.PlaylistEntries != null)
                {
                    PlaylistLength = vm.VideoData.PlaylistEntries.Length;
                }
                else
                {
                    PlaylistLength = -1;
                }
            }
        }

        // текстовые поля (ввод пользователя)
        private string? _startText;
        public string? StartText
        {
            get => _startText;
            set
            {
                if (SetProperty(ref _startText, value))
                {
                    ValidateAll();
                    // при изменении входных данных пересматриваем can-execute
                    ApplyRangeCommand.NotifyCanExecuteChanged();
                }
            }
        }

        private string? _endText;
        public string? EndText
        {
            get => _endText;
            set
            {
                if (SetProperty(ref _endText, value))
                {
                    ValidateAll();
                    ApplyRangeCommand.NotifyCanExecuteChanged();
                }
            }
        }

        private string? _countText;
        public string? CountText
        {
            get => _countText;
            set
            {
                if (SetProperty(ref _countText, value))
                {
                    ValidateAll();
                    ApplyRangeCommand.NotifyCanExecuteChanged();
                }
            }
        }

        // ошибки (показываем в ui)
        private string? _startError;
        public string? StartError
        {
            get => _startError;
            private set => SetProperty(ref _startError, value);
        }

        private string? _endError;
        public string? EndError
        {
            get => _endError;
            private set => SetProperty(ref _endError, value);
        }

        private string? _countError;
        public string? CountError
        {
            get => _countError;
            private set => SetProperty(ref _countError, value);
        }

        // информационная строка
        private string? _videoPlaylistInfoText;
        public string? VideoPlaylistInfoText
        {
            get => _videoPlaylistInfoText;
            private set => SetProperty(ref _videoPlaylistInfoText, value);
        }

        // результат (после apply): вычисленные индексы в той же базе, что и yt-dlp принимает
        public int? FinalStartIndex { get; private set; }
        public int? FinalEndIndex { get; private set; }

        // событие/колбэк, если нужно уведомить родителя
        public Action<int?, int?>? OnRangeApplied { get; set; }

        // выполнение применения диапазона
        [RelayCommand(CanExecute = nameof(CanApply))]
        private void ApplyRange()
        {
            // пересчёт final индексов на основе введённых данных
            FinalStartIndex = null;
            FinalEndIndex = null;

            int min = IsZeroBasedIndex ? 0 : 1;
            int max = Math.Max(min, PlaylistLength == 0 ? min : (IsZeroBasedIndex ? PlaylistLength - 1 : PlaylistLength));

            if (!string.IsNullOrWhiteSpace(StartText) && int.TryParse(StartText.Trim(), out var s))
            {
                FinalStartIndex = Math.Clamp(s, min, max);
            }

            if (!string.IsNullOrWhiteSpace(EndText) && int.TryParse(EndText.Trim(), out var e))
            {
                FinalEndIndex = Math.Clamp(e, min, max);
            }

            if (!string.IsNullOrWhiteSpace(CountText) && int.TryParse(CountText.Trim(), out var c))
            {
                if (FinalStartIndex.HasValue)
                {
                    FinalEndIndex = Math.Min(max, FinalStartIndex.Value + c - 1);
                }
                else
                {
                    // если нет start, начнём с min
                    FinalStartIndex = min;
                    FinalEndIndex = Math.Min(max, FinalStartIndex.Value + c - 1);
                }
            }

            // если указан только start и нет ни end ни count — считаем, что качаем только 1 видео
            if (FinalStartIndex.HasValue && !FinalEndIndex.HasValue)
            {
                FinalEndIndex = PlaylistLength;
            }

            // если вообще ничего не указано — finalstartindex остаётся null (значит качаем всё)
            // вызов колбэка/уведомление родителя
            OnRangeApplied?.Invoke(FinalStartIndex, FinalEndIndex);
        }

        // can execute для apply
        private bool CanApply()
        {
            return string.IsNullOrEmpty(StartError) &&
                   string.IsNullOrEmpty(EndError) &&
                   string.IsNullOrEmpty(CountError);
        }

        // validation helpers
        private void UpdatePlaylistInfoTextAndValidate()
        {
            UpdatePlaylistInfoText();
            ValidateAll();
        }

        private void UpdatePlaylistInfoText()
        {
            VideoPlaylistInfoText = $"всего в плейлисте: {PlaylistLength}";
        }

        private void ValidateAll()
        {
            ValidateStart();
            ValidateEnd();
            ValidateCount();

            // уведомляем команду, что состояние can-execute могло поменяться
            ApplyRangeCommand.NotifyCanExecuteChanged();
        }

        private void ValidateStart()
        {
            StartError = null;
            if (string.IsNullOrWhiteSpace(StartText))
            {
                // допускаем null (тогда начальный индекс не задан)
                return;
            }

            if (!int.TryParse(StartText.Trim(), out var v))
            {
                StartError = "нужно число";
                return;
            }

            int min = IsZeroBasedIndex ? 0 : 1;
            int max = Math.Max(min, PlaylistLength == 0 ? min : (IsZeroBasedIndex ? PlaylistLength - 1 : PlaylistLength));

            if (v < min || v > max)
                StartError = $"ожидается число в диапазоне {min}…{max}";
        }

        private void ValidateEnd()
        {
            EndError = null;
            if (string.IsNullOrWhiteSpace(EndText)) return;

            if (!int.TryParse(EndText.Trim(), out var v))
            {
                EndError = "нужно число";
                return;
            }

            int min = IsZeroBasedIndex ? 0 : 1;
            int max = Math.Max(min, PlaylistLength == 0 ? min : (IsZeroBasedIndex ? PlaylistLength - 1 : PlaylistLength));
            if (v < min || v > max)
            {
                EndError = $"ожидается число в диапазоне {min}…{max}";
                return;
            }

            // если есть start — проверим порядок
            if (int.TryParse(StartText ?? "", out var s))
            {
                if (s > v) EndError = "конец должен быть >= начала";
            }
        }

        private void ValidateCount()
        {
            CountError = null;
            if (string.IsNullOrWhiteSpace(CountText)) return;

            if (!int.TryParse(CountText.Trim(), out var c))
            {
                CountError = "нужно число";
                return;
            }

            if (c < 1) CountError = "должно быть >= 1";

            // если задан count и задан end — обычно логично запретить оба одновременно
            if (!string.IsNullOrWhiteSpace(CountText) && !string.IsNullOrWhiteSpace(EndText))
            {
                CountError = "укажи либо конечный индекс, либо количество, но не оба";
            }

            int.TryParse(StartText, out int start);

            start = start <= 0 ? 1 : start;

            if (start < PlaylistLength && start + c - 1 > PlaylistLength)
            {
                CountError = "Превышен размер плейлиста";
            }

            // заметка: можно требовать start при указании count, но здесь оставляем опционально
        }
    }
}
