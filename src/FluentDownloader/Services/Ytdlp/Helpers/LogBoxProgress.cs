using FluentDownloader.Dialogs;
using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace FluentDownloader.Services.Ytdlp.Helpers
{
    public class LogBoxProgress : IProgress<string>, IDisposable
    {
        private readonly IDownloadDependencies _downloadDependencies;
        private readonly IProgressBar _progressBar;
        private readonly SynchronizationContext _context;
        private bool _isDownloading;
        private double _lastProgress = -1;
        private string? _lastMessage;
        private readonly Timer _updateTimer;
        private readonly object _lock = new();

        /// <summary>
        /// Регулярное выражение для извлечения процента загрузки.
        /// </summary>
        private static readonly Regex DownloadRegex = new(@"\[download\]\s+(100|\d{1,2}\.\d{1,2})%\s+of\s+~?\s*\d+(?:\.\d+)?.*", RegexOptions.Compiled);

        public LogBoxProgress(IDialogService dialogService,
            IDownloadDependencies downloadDependencies,
            IProgressBar progressBar)
        {
            _downloadDependencies = downloadDependencies;
            _progressBar = progressBar;
            _context = SynchronizationContext.Current ?? throw new InvalidOperationException("Must be created in UI thread");

            _updateTimer = new Timer(UpdateUI, null, 0, App.AppSettings.Download.LogBoxUpdateRateMs);
        }

        private bool _argumentsLogged = false; // Флаг для первой строки

        public void Report(string value)
        {
            lock (_lock)
            {
                if (TryGetDownloadProgress(value, out double progress))
                {
                    if (Math.Abs(progress - _lastProgress) < 0.5 && progress != 100.0d)
                        return;

                    _lastMessage = value;
                    _lastProgress = progress;
                }
                else if (!_argumentsLogged && value.StartsWith("Arguments:"))
                {
                    if (App.AppSettings.Download.VerboseYtdlpOptions)
                    {
                        _lastMessage = value;
                        _argumentsLogged = true; // Устанавливаем флаг, больше не проверяем
                        return;
                    }
                }
                else
                {
                    _lastMessage = $"[ytdlp | {DateTime.Now:HH:mm:ss}] {value}";
                }
            }
        }

        public void FlushUI()
        {
            UpdateUI(null);
        }

        /// <summary>
        /// Периодически обновляет UI.
        /// </summary>
        private void UpdateUI(object? state)
        {
            string? message;
            double progress;

            lock (_lock)
            {
                if (_lastMessage == null)
                    return;

                message = _lastMessage;
                progress = _lastProgress;
                _lastMessage = null; // Очищаем после обработки
            }

            _context.Post(_ =>
            {
                if (TryGetDownloadProgress(message, out double parsedProgress))
                {
                    if (_isDownloading)
                    {
                        _downloadDependencies.LogsTextBoxOverrideLine(message);
                    }
                    else
                    {
                        _downloadDependencies.LogsTextBoxWriteLine(message);
                        _isDownloading = true;
                    }

                    _progressBar.UpdateInstallProgress((int)progress);
                }
                else
                {
                    _isDownloading = false;
                    _downloadDependencies.LogsTextBoxWriteLine(message);
                }
            }, null);
        }

        /// <summary>
        /// Извлекает процент загрузки из строки.
        /// </summary>
        private static bool TryGetDownloadProgress(string input, out double progress)
        {
            progress = 0;
            var match = DownloadRegex.Match(input);
            return match.Success && double.TryParse(match.Groups[1].Value, NumberStyles.Float, CultureInfo.InvariantCulture, out progress);
        }

        /// <summary>
        /// Освобождает ресурсы при завершении работы.
        /// </summary>
        public void Dispose()
        {
            _updateTimer.Dispose();
        }
    }
}
