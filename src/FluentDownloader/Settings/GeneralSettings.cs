using Windows.Storage;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace FluentDownloader.Settings;

public class GeneralSettings : INotifyPropertyChanged
{
    private readonly ApplicationDataContainer _localSettings;

    public event PropertyChangedEventHandler? PropertyChanged;

    public GeneralSettings(ApplicationDataContainer localSettings)
    {
        _localSettings = localSettings;
    }

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public string LastPeekedOutputPath
    {
        get => _localSettings.Values.TryGetValue("OutputPath", out object? value) && value is string path
                    ? path
                    : string.Empty;
        set
        {
            if (LastPeekedOutputPath == value)
                return;

            _localSettings.Values["OutputPath"] = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LastPeekedOutputPath)));
        }
    }
}
