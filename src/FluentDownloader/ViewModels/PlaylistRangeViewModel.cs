using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace FluentDownloader.ViewModels
{
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

        // --- конструктор
        public PlaylistRangeViewModel(VideoDownloadViewModel? downloadViewModel = null)
        {
            if (downloadViewModel != null)
            {
                downloadViewModel.PropertyChanged += DownloadViewModel_PropertyChanged;
            }

            UpdatePlaylistInfoText();
        }

        private void DownloadViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(VideoDownloadViewModel.VideoData))
            {
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

        // --- новое поле: PlaylistItems (список/диапазоны через запятую и дефис)
        private string? _playlistItems;
        public string? PlaylistItems
        {
            get => _playlistItems;
            set
            {
                if (SetProperty(ref _playlistItems, value))
                {
                    ValidateAll();
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

        // Ошибка для PlaylistItems
        private string? _playlistItemsError;
        public string? PlaylistItemsError
        {
            get => _playlistItemsError;
            private set => SetProperty(ref _playlistItemsError, value);
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

        // дополнительно: итоговый набор индексов из PlaylistItems (expanded), чтобы родитель мог их использовать
        public IReadOnlyList<int> FinalPlaylistItems { get; private set; } = Array.Empty<int>();

        // событие/колбэк, если нужно уведомить родителя
        public Action<int?, int?>? OnRangeApplied { get; set; }

        // выполнение применения диапазона
        [RelayCommand(CanExecute = nameof(CanApply))]
        private void ApplyRange()
        {
            // пересчёт final индексов на основе введённых данных
            FinalStartIndex = null;
            FinalEndIndex = null;
            FinalPlaylistItems = Array.Empty<int>();

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
                    FinalStartIndex = min;
                    FinalEndIndex = Math.Min(max, FinalStartIndex.Value + c - 1);
                }
            }

            // если указан только start и нет ни end ни count — считаем, что качаем только 1 видео
            if (FinalStartIndex.HasValue && !FinalEndIndex.HasValue)
            {
                FinalEndIndex = PlaylistLength;
            }

            // обработаем PlaylistItems — если валидны, развернём в список индексов
            var parseResult = TryParsePlaylistItems(PlaylistItems ?? string.Empty, out var parsed, out var parseError);
            if (parseResult)
            {
                FinalPlaylistItems = parsed.OrderBy(x => x).ToArray();
            }
            else
            {
                FinalPlaylistItems = Array.Empty<int>();
            }

            // вызов колбэка/уведомление родителя
            OnRangeApplied?.Invoke(FinalStartIndex, FinalEndIndex);
        }

        // can execute для apply
        private bool CanApply()
        {
            return string.IsNullOrEmpty(StartError) &&
                   string.IsNullOrEmpty(EndError) &&
                   string.IsNullOrEmpty(CountError) &&
                   string.IsNullOrEmpty(PlaylistItemsError);
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
            ValidatePlaylistItems();

            // уведомляем команду, что состояние can-execute могло поменяться
            ApplyRangeCommand.NotifyCanExecuteChanged();
        }

        private void ValidateStart()
        {
            StartError = null;
            if (string.IsNullOrWhiteSpace(StartText))
            {
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
        }

        // ===========================
        // PlaylistItems: парсинг и валидация
        // ===========================

        // Формат: пустая строка (означает отсутствие) или список элементов, разделённых запятой.
        // Каждый элемент — либо одно число, либо диапазон "a-b" (a и b включительно).
        // Пример: "1,2,3-5,8"
        private static readonly Regex _tokenRegex = new Regex(@"^\s*(\d+)(\s*-\s*(\d+))?\s*$", RegexOptions.Compiled);

        private void ValidatePlaylistItems()
        {
            PlaylistItemsError = null;

            if (string.IsNullOrWhiteSpace(PlaylistItems))
            {
                // пусто — допустимо
                return;
            }

            if (!TryParsePlaylistItems(PlaylistItems, out var expanded, out var error))
            {
                PlaylistItemsError = error;
                return;
            }

            // Теперь проверим границы по длине плейлиста
            int min = IsZeroBasedIndex ? 0 : 1;
            int max = Math.Max(min, PlaylistLength == 0 ? min : (IsZeroBasedIndex ? PlaylistLength - 1 : PlaylistLength));

            var outOfRange = expanded.Where(i => i < min || i > max).ToList();
            if (outOfRange.Any())
            {
                PlaylistItemsError = $"Индексы вне диапазона {min}…{max}: {string.Join(",", outOfRange.Take(10))}";
                return;
            }

            // Проверим на дубликаты (после развёртки дубликатов не будет — расширение даёт Set), но на всякий случай
            if (expanded.Count != expanded.Distinct().Count())
            {
                PlaylistItemsError = "Есть дублирующиеся индексы";
                return;
            }

            // Проверим пересечение с текущим диапазоном, вычисленным по Start/End/Count.
            var implied = ComputeImpliedRange();
            if (implied.start.HasValue && implied.end.HasValue)
            {
                int s = implied.start.Value;
                int e = implied.end.Value;
                var intersection = expanded.Where(i => i >= s && i <= e).ToList();
                if (intersection.Any())
                {
                    PlaylistItemsError = $"Нельзя указывать номера, пересекающиеся с выбранным диапазоном {s}…{e}: {string.Join(",", intersection.Take(10))}";
                    return;
                }
            }

            // всё ок
            PlaylistItemsError = null;
        }

        // Попытка распарсить строку в набор индексов (expanded). Возвращает false и сообщение об ошибке если не удалось.
        private bool TryParsePlaylistItems(string input, out List<int> expanded, out string? error)
        {
            expanded = new List<int>();
            error = null;

            if (string.IsNullOrWhiteSpace(input))
            {
                return true;
            }

            var tokens = input.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Length == 0)
            {
                error = "пустой список";
                return false;
            }

            var resultSet = new HashSet<int>();
            int min = IsZeroBasedIndex ? 0 : 1;

            for (int ti = 0; ti < tokens.Length; ti++)
            {
                var t = tokens[ti].Trim();
                if (string.IsNullOrEmpty(t))
                {
                    error = "пустой элемент в списке";
                    return false;
                }

                var m = _tokenRegex.Match(t);
                if (!m.Success)
                {
                    error = $"некорректный элемент: '{t}' (ожидается число или диапазон a-b)";
                    return false;
                }

                // group1 = first number, group3 = second number (если есть)
                if (!int.TryParse(m.Groups[1].Value, out var a))
                {
                    error = $"некорректное число: '{m.Groups[1].Value}'";
                    return false;
                }

                if (m.Groups[3].Success)
                {
                    if (!int.TryParse(m.Groups[3].Value, out var b))
                    {
                        error = $"некорректное число: '{m.Groups[3].Value}'";
                        return false;
                    }

                    if (a > b)
                    {
                        error = $"диапазон неверен: {a}-{b} (левое число должно быть <= правого)";
                        return false;
                    }

                    // развернём диапазон
                    for (int x = a; x <= b; x++)
                    {
                        resultSet.Add(x);
                    }
                }
                else
                {
                    resultSet.Add(a);
                }
            }

            expanded = resultSet.OrderBy(x => x).ToList();
            return true;
        }

        // Вычисляет подразумеваемый диапазон по StartText/EndText/CountText без изменения FinalStart/FinalEnd.
        // Возвращает (start?.end?) где значения в той же базе индексов.
        private (int? start, int? end) ComputeImpliedRange()
        {
            int min = IsZeroBasedIndex ? 0 : 1;
            int max = Math.Max(min, PlaylistLength == 0 ? min : (IsZeroBasedIndex ? PlaylistLength - 1 : PlaylistLength));

            int? sIdx = null;
            int? eIdx = null;

            if (!string.IsNullOrWhiteSpace(StartText) && int.TryParse(StartText.Trim(), out var s))
            {
                sIdx = Math.Clamp(s, min, max);
            }

            if (!string.IsNullOrWhiteSpace(EndText) && int.TryParse(EndText.Trim(), out var e))
            {
                eIdx = Math.Clamp(e, min, max);
            }

            if (!string.IsNullOrWhiteSpace(CountText) && int.TryParse(CountText.Trim(), out var c))
            {
                if (sIdx.HasValue)
                {
                    eIdx = Math.Min(max, sIdx.Value + c - 1);
                }
                else
                {
                    sIdx = min;
                    eIdx = Math.Min(max, sIdx.Value + c - 1);
                }
            }

            // если указан только start и нет ни end ни count — считаем, что качаем 1 видео (как и в ApplyRange)
            if (sIdx.HasValue && !eIdx.HasValue)
            {
                eIdx = sIdx.Value;
            }

            return (sIdx, eIdx);
        }
    }
}
