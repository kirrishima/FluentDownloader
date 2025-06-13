using FluentDownloader.Models;
using Windows.Storage;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace FluentDownloader.Settings;

public class DownloadSettings : INotifyPropertyChanged
{
    private readonly ApplicationDataContainer _localSettings;

    public event PropertyChangedEventHandler? PropertyChanged;

    public DownloadSettings(ApplicationDataContainer localSettings)
    {
        _localSettings = localSettings;
    }

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private string? _ytDlpExePath;
    private bool _isYtDlpExePathLoaded = false;

    public string? YtDlpExePath
    {
        get
        {
            if (!_isYtDlpExePathLoaded)
            {
                if (_localSettings.Values.TryGetValue("YtDlpExePath", out object? value))
                {
                    _ytDlpExePath = (string)value;
                }
                else
                {
                    _ytDlpExePath = string.Empty; // значение по умолчанию
                }
                _isYtDlpExePathLoaded = true;
            }
            return _ytDlpExePath;
        }
        set
        {
            if (_isYtDlpExePathLoaded && _ytDlpExePath == value)
                return;

            _ytDlpExePath = value;
            _isYtDlpExePathLoaded = true;
            _localSettings.Values["YtDlpExePath"] = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(YtDlpExePath)));
        }
    }


    private string? _ffmpegExePath;
    private bool _isFfmpegExePathLoaded = false;

    public string? FfmpegExePath
    {
        get
        {
            if (!_isFfmpegExePathLoaded)
            {
                if (_localSettings.Values.TryGetValue("FfmpegExePath", out object? value))
                {
                    _ffmpegExePath = (string)value;
                }
                else
                {
                    _ffmpegExePath = string.Empty; // значение по умолчанию
                }
                _isFfmpegExePathLoaded = true;
            }
            return _ffmpegExePath;
        }
        set
        {
            if (_isFfmpegExePathLoaded && _ffmpegExePath == value)
                return;

            _ffmpegExePath = value;
            _isFfmpegExePathLoaded = true;
            _localSettings.Values["FfmpegExePath"] = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FfmpegExePath)));
        }
    }

    private bool _verboseYtdlpOptions;
    private bool _isVerboseYtdlpOptionsLoaded = false;

    public bool VerboseYtdlpOptions
    {
        get
        {
            if (!_isVerboseYtdlpOptionsLoaded)
            {
                if (_localSettings.Values.TryGetValue("VerboseYtdlpOptions", out object? value))
                {
                    _verboseYtdlpOptions = (bool)value;
                }
                else
                {
                    _verboseYtdlpOptions = false;
                }
                _isVerboseYtdlpOptionsLoaded = true;
            }
            return _verboseYtdlpOptions;
        }
        set
        {
            if (_isVerboseYtdlpOptionsLoaded && _verboseYtdlpOptions == value)
                return;

            _verboseYtdlpOptions = value;
            _isVerboseYtdlpOptionsLoaded = true;
            _localSettings.Values["VerboseYtdlpOptions"] = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(VerboseYtdlpOptions)));
        }
    }

    private int _ytdlpConcurrentFragments;
    private bool _isYtdlpConcurrentFragmentsLoaded = false;

    public int YtdlpConcurrentFragments
    {
        get
        {
            if (!_isYtdlpConcurrentFragmentsLoaded)
            {
                if (_localSettings.Values.TryGetValue("YtdlpConcurrentFragments", out object? value))
                {
                    _ytdlpConcurrentFragments = (int)value;
                }
                else
                {
                    _ytdlpConcurrentFragments = 6;
                }
                _isYtdlpConcurrentFragmentsLoaded = true;
            }
            return _ytdlpConcurrentFragments;
        }
        set
        {
            if (_isYtdlpConcurrentFragmentsLoaded && _ytdlpConcurrentFragments == value)
                return;

            _ytdlpConcurrentFragments = value;
            _isYtdlpConcurrentFragmentsLoaded = true;
            _localSettings.Values["YtdlpConcurrentFragments"] = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(YtdlpConcurrentFragments)));
        }
    }

    private bool _useRateLimit;
    private bool _isUseRateLimitLoaded = false;

    public bool UseRateLimit
    {
        get
        {
            if (!_isUseRateLimitLoaded)
            {
                if (_localSettings.Values.TryGetValue("UseRateLimit", out object? value))
                {
                    _useRateLimit = (bool)value;
                }
                else
                {
                    _useRateLimit = false;
                }
                _isUseRateLimitLoaded = true;
            }
            return _useRateLimit;
        }
        set
        {
            if (_isUseRateLimitLoaded && _useRateLimit == value)
                return;

            _useRateLimit = value;
            _isUseRateLimitLoaded = true;
            _localSettings.Values["UseRateLimit"] = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UseRateLimit)));
        }
    }

    private long _rateLimitInBytes;
    private bool _isRateLimitLoaded = false;

    public long RateLimitInBytes
    {
        get
        {
            if (!_isRateLimitLoaded)
            {
                if (_localSettings.Values.TryGetValue("RateLimitInBytes", out object? value))
                {
                    _rateLimitInBytes = Convert.ToInt64(value);
                }
                else
                {
                    _rateLimitInBytes = -1;
                }
                _isRateLimitLoaded = true;
            }
            return _rateLimitInBytes;
        }
        set
        {
            if (_isRateLimitLoaded && _rateLimitInBytes == value)
                return;

            _rateLimitInBytes = value;
            _isRateLimitLoaded = true;
            _localSettings.Values["RateLimitInBytes"] = value;

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RateLimitInBytes)));
        }
    }

    private string? _fileOutputTemplate;

    public string FileOutputTemplate
    {
        get => _fileOutputTemplate ??= LoadFileOutputTemplate();
        set
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                _fileOutputTemplate = value;
                _localSettings.Values["YtdlpFileOutputTemplate"] = value;
            }
        }
    }

    public string RestoreDefaultFileOutputTemplate()
    {
        FileOutputTemplate = "%(title)s [%(resolution)s].%(ext)s";
        return FileOutputTemplate;
    }

    private string LoadFileOutputTemplate()
    {
        return _localSettings.Values["YtdlpFileOutputTemplate"] as string ?? "%(title)s [%(resolution)s].%(ext)s";
    }

    private int _logBoxUpdateRateMs;
    private bool _isLogBoxUpdateRateMsLoaded = false;

    public int LogBoxUpdateRateMs
    {
        get
        {
            if (!_isLogBoxUpdateRateMsLoaded)
            {
                if (_localSettings.Values.TryGetValue("LogBoxUpdateRateMs", out object? value))
                {
                    _logBoxUpdateRateMs = (int)value;
                }
                else
                {
                    _logBoxUpdateRateMs = 100;
                }
                _isLogBoxUpdateRateMsLoaded = true;
            }
            return _logBoxUpdateRateMs;
        }
        set
        {
            if (_isLogBoxUpdateRateMsLoaded && _logBoxUpdateRateMs == value)
                return;

            _logBoxUpdateRateMs = value;
            _isLogBoxUpdateRateMsLoaded = true;
            _localSettings.Values["LogBoxUpdateRateMs"] = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LogBoxUpdateRateMs)));
        }
    }

    private bool _useProxy;
    private bool _isUseProxyLoaded;

    public bool UseProxy
    {
        get
        {
            if (!_isUseProxyLoaded)
            {
                if (_localSettings.Values.TryGetValue("UseProxy", out object? value) && value is bool boolValue)
                {
                    _useProxy = boolValue;
                }
                else
                {
                    _useProxy = false;
                }
                _isUseProxyLoaded = true;
            }
            return _useProxy;
        }
        set
        {
            if (_isUseProxyLoaded && _useProxy == value)
                return;

            _useProxy = value;
            _isUseProxyLoaded = true;
            _localSettings.Values["UseProxy"] = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UseProxy)));
        }
    }


    private string _proxy = string.Empty;
    private bool _isProxyLoaded;

    public string? Proxy
    {
        get
        {
            if (!_isProxyLoaded)
            {
                _proxy = _localSettings.Values["Proxy"] as string ?? string.Empty;
                _isProxyLoaded = true;
            }
            return _proxy;
        }
        set
        {
            if (_isProxyLoaded && _proxy == value)
                return;

            _proxy = value ?? string.Empty;
            _isProxyLoaded = true;
            _localSettings.Values["Proxy"] = _proxy;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Proxy)));
        }
    }

    private bool _showVideoThumbnailAndTitle;
    private bool _isShowVideoThumbnailAndTitleLoaded = false;

    public bool ShowVideoThumbnailAndTitle
    {
        get
        {
            if (!_isShowVideoThumbnailAndTitleLoaded)
            {
                if (_localSettings.Values.TryGetValue("ShowVideoThumbnailAndTitle", out object? value))
                {
                    _showVideoThumbnailAndTitle = (bool)value;
                }
                else
                {
                    _showVideoThumbnailAndTitle = true;
                }
                _isShowVideoThumbnailAndTitleLoaded = true;
            }
            return _showVideoThumbnailAndTitle;
        }
        set
        {
            if (_isShowVideoThumbnailAndTitleLoaded && _showVideoThumbnailAndTitle == value)
                return;

            _showVideoThumbnailAndTitle = value;
            _isShowVideoThumbnailAndTitleLoaded = true;
            _localSettings.Values["ShowVideoThumbnailAndTitle"] = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ShowVideoThumbnailAndTitle)));
        }
    }
}
