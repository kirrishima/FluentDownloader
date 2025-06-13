using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;
using Microsoft.UI.Xaml;

namespace FluentDownloader.ViewModels
{
    public class ThemeViewModel : INotifyPropertyChanged
    {
        private static readonly ThemeViewModel _instance = new();
        public static ThemeViewModel Instance => _instance;

        private ElementTheme _currentTheme = ElementTheme.Light;

        public ElementTheme CurrentTheme
        {
            get => _currentTheme;
            set
            {
                _currentTheme = value;
                OnPropertyChanged(nameof(CurrentTheme));
            }
        }

        public ICommand ToggleThemeCommand { get; }

        private ThemeViewModel()
        {
            ToggleThemeCommand = new RelayCommand(ToggleTheme);
            var savedTheme = App.AppSettings.Appearance.AppTheme;
            _currentTheme = savedTheme;
            OnPropertyChanged(nameof(CurrentTheme));
        }

        private void ToggleTheme()
        {
            CurrentTheme = CurrentTheme == ElementTheme.Dark ? ElementTheme.Light : ElementTheme.Dark;
            App.AppSettings.Appearance.AppTheme = CurrentTheme;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
