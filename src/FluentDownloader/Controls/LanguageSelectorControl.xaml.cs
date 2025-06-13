using FluentDownloader.Helpers;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;
using System.Linq;

namespace FluentDownloader.Controls
{
    public sealed partial class LanguageSelectorControl : UserControl
    {
        // ����, �����������, ��� ������� ������������� ������������� ���������� ����� ����� �����.
        // �� ��������� true. �������� ����� ��������������.
        public bool AutoRestartOnLanguageChange
        {
            get => (bool)GetValue(AutoRestartOnLanguageChangeProperty);
            set => SetValue(AutoRestartOnLanguageChangeProperty, value);
        }

        public static readonly DependencyProperty AutoRestartOnLanguageChangeProperty =
            DependencyProperty.Register(nameof(AutoRestartOnLanguageChange), typeof(bool),
                typeof(LanguageSelectorControl), new PropertyMetadata(true));

        // ��������� ���� (����� ������� ��������� ��������� ��� ������������� ����������)
        public Language SelectedLanguage
        {
            get => (Language)GetValue(SelectedLanguageProperty);
            set => SetValue(SelectedLanguageProperty, value);
        }

        public static readonly DependencyProperty SelectedLanguageProperty =
            DependencyProperty.Register(nameof(SelectedLanguage), typeof(Language),
                typeof(LanguageSelectorControl), new PropertyMetadata(null));

        // �������, ������������, ��� ���� ������.
        public event RoutedEventHandler? LanguageChanged;

        private bool _initialized;

        public LanguageSelectorControl()
        {
            this.InitializeComponent();
            InitializeLanguageSelector();

            _initialized = true;
        }

        private void InitializeLanguageSelector()
        {
            // ����� ����� ������� ������ �������������� ������ ����-������ (��������, � ������� ��� � ������ �����������)
            var supportedLanguages = new List<Language>
            {
                new Language("ru-RU", "�������"),
                new Language("en-US", "English")
            };

            LanguageComboBox.ItemsSource = supportedLanguages;

            // ���������� ������� ���� �� ������ ApplicationLanguages.PrimaryLanguageOverride
            var currentLanguage = supportedLanguages
                .FirstOrDefault(l => l.Code == Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride)
                ?? supportedLanguages.First();

            LanguageComboBox.SelectedItem = currentLanguage;
            SelectedLanguage = currentLanguage;
        }

        private void LanguageComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LanguageComboBox.SelectedItem is Language selectedLanguage && _initialized)
            {
                SelectedLanguage = selectedLanguage;
                Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = selectedLanguage.Code;

                LanguageChanged?.Invoke(this, new RoutedEventArgs());

                LanguageChangedManager.OnLanguageChanged();
                // ���� �������� �������������� ���������� (��������, ����������), �� ��������� ���.
                if (AutoRestartOnLanguageChange)
                {
                    // ����� ����� �������� ������������� ��� ������, �����������, ����� �� ������ ������������� ����������.
                    Microsoft.Windows.AppLifecycle.AppInstance.Restart("");
                }
            }
        }
    }
}
