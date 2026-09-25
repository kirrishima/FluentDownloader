using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace FluentDownloader.Pages.Settings;

public sealed partial class CustomYtdlpParamsPage : Page, INotifyPropertyChanged
{
    public ObservableCollection<YtdlpOptionItem> Options { get; } = new();

    private string _newKey = "";
    public string NewKey
    {
        get => _newKey;
        set => SetProperty(ref _newKey, value);
    }

    private string _newValue = "";
    public string NewValue
    {
        get => _newValue;
        set => SetProperty(ref _newValue, value);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public CustomYtdlpParamsPage()
    {
        InitializeComponent();
        Load();
    }

    private void Load()
    {
        Options.Clear();
        foreach (var kv in App.AppSettings.Download.CustomYtdlpOptions)
            Options.Add(new YtdlpOptionItem { Key = kv.Key, Value = kv.Value });
    }

    private void Save()
    {
        App.AppSettings.Download.CustomYtdlpOptions = Options
            .Where(x => !string.IsNullOrWhiteSpace(x.Key))
            .Select(x => new KeyValuePair<string, string>(x.Key, x.Value))
            .ToList();
    }

    private void Add_Click(object sender, RoutedEventArgs e)
    {
        var key = NewKey.Trim();

        if (string.IsNullOrEmpty(NewKey) || NewKey.Contains(' '))
        {
            return;
        }

        var existing = Options.FirstOrDefault(x =>
            string.Equals(x.Key, key, StringComparison.OrdinalIgnoreCase));

        if (existing != null)
        {
            existing.Value = NewValue;
        }
        else
        {
            Options.Add(new YtdlpOptionItem
            {
                Key = key,
                Value = NewValue
            });
        }

        NewKey = "";
        NewValue = "";

        Save();
    }

    private void Edit_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is YtdlpOptionItem item)
        {
            item.BeginEdit();
        }
    }

    private void SaveItem_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button btn || btn.Tag is not YtdlpOptionItem item)
            return;

        var newKey = item.Key.Trim();
        if (string.IsNullOrWhiteSpace(newKey))
        {
            item.CancelEdit();
            return;
        }

        var duplicateExists = Options.Any(x =>
            !ReferenceEquals(x, item) &&
            string.Equals(x.Key, newKey, StringComparison.OrdinalIgnoreCase));

        if (duplicateExists)
        {
            item.CancelEdit();
            return;
        }

        item.Key = newKey;
        item.CommitEdit();
        Save();
    }

    private void CancelItem_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is YtdlpOptionItem item)
        {
            item.CancelEdit();
        }
    }

    private void Delete_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button btn || btn.Tag is not YtdlpOptionItem item)
            return;

        Options.Remove(item);
        Save();
    }

    private bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return false;

        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        return true;
    }
}

public class YtdlpOptionItem : INotifyPropertyChanged
{
    private string _key = "";
    public string Key
    {
        get => _key;
        set
        {
            if (_key == value) return;
            _key = value;
            OnPropertyChanged();
        }
    }

    private string _value = "";
    public string Value
    {
        get => _value;
        set
        {
            if (_value == value) return;
            _value = value;
            OnPropertyChanged();
        }
    }

    private bool _isEditing;
    public bool IsEditing
    {
        get => _isEditing;
        private set
        {
            if (_isEditing == value) return;
            _isEditing = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(NotIsEditing));
        }
    }

    public bool NotIsEditing => !IsEditing;

    private string _backupKey = "";
    private string _backupValue = "";

    public void BeginEdit()
    {
        _backupKey = Key;
        _backupValue = Value;
        IsEditing = true;
    }

    public void CancelEdit()
    {
        Key = _backupKey;
        Value = _backupValue;
        IsEditing = false;
    }

    public void CommitEdit()
    {
        _backupKey = Key;
        _backupValue = Value;
        IsEditing = false;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}