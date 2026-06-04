using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace FluentDownloader.Pages.Settings
{
    public sealed partial class CustomYtdlpParamsPage : Page, INotifyPropertyChanged
    {
        public ObservableCollection<YtdlpOptionItem> Options { get; } = new();

        public string NewKey { get; set; } = "";
        public string NewValue { get; set; } = "";

        public event PropertyChangedEventHandler? PropertyChanged;
        private bool IsInitialized { get; init; } = false;
        private bool _isLoading;
        public CustomYtdlpParamsPage()
        {
            InitializeComponent();

            Load();

            Options.CollectionChanged += Options_CollectionChanged;

            IsInitialized = true;
        }

        private void Options_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (_isLoading) return;

            Save();
        }

        private void Load()
        {
            _isLoading = true;

            Options.Clear();

            var dict = App.AppSettings.Download.CustomYtdlpOptions;

            foreach (var kv in dict)
            {
                var item = new YtdlpOptionItem
                {
                    Key = kv.Key,
                    Value = kv.Value
                };

                item.PropertyChanged += Item_PropertyChanged;

                Options.Add(item);
            }

            _isLoading = false;
        }

        private void Item_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (_isLoading) return;

            Save();
        }

        private void Save()
        {
            if (!IsInitialized) return;

            var dict = Options
                .Where(x => !string.IsNullOrWhiteSpace(x.Key))
                .GroupBy(x => x.Key)
                .ToDictionary(g => g.Key, g => g.Last().Value);

            App.AppSettings.Download.CustomYtdlpOptions = new ReadOnlyDictionary<string, string>(dict);
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NewKey))
                return;

            // защита от дублей
            var existing = Options.FirstOrDefault(x => x.Key == NewKey);
            if (existing != null)
            {
                existing.Value = NewValue;
            }
            else
            {
                var item = new YtdlpOptionItem
                {
                    Key = NewKey,
                    Value = NewValue
                };

                item.PropertyChanged += Item_PropertyChanged;

                Options.Add(item);
            }

            NewKey = "";
            NewValue = "";

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NewKey)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NewValue)));

            Save();
        }

        private void Delete(YtdlpOptionItem item)
        {
            Options.Remove(item);
            Save();
        }

        private void ValueChanged(object sender, TextChangedEventArgs e)
        {
            if (_isLoading) return;

            Save();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is YtdlpOptionItem item)
            {
                Delete(item);
            }
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
                _value = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}