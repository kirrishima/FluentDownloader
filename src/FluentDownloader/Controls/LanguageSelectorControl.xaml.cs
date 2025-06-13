using FluentDownloader.Helpers;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;
using System.Linq;

namespace FluentDownloader.Controls
{
    public sealed partial class LanguageSelectorControl : UserControl
    {
        // ‘лаг, указывающий, что контрол автоматически перезапускает приложение после смены €зыка.
        // ѕо умолчанию true. –одитель может переопределить.
        public bool AutoRestartOnLanguageChange
        {
            get => (bool)GetValue(AutoRestartOnLanguageChangeProperty);
            set => SetValue(AutoRestartOnLanguageChangeProperty, value);
        }

        public static readonly DependencyProperty AutoRestartOnLanguageChangeProperty =
            DependencyProperty.Register(nameof(AutoRestartOnLanguageChange), typeof(bool),
                typeof(LanguageSelectorControl), new PropertyMetadata(true));

        // ¬ыбранный €зык (можно сделать зависимым свойством дл€ двустороннего св€зывани€)
        public Language SelectedLanguage
        {
            get => (Language)GetValue(SelectedLanguageProperty);
            set => SetValue(SelectedLanguageProperty, value);
        }

        public static readonly DependencyProperty SelectedLanguageProperty =
            DependencyProperty.Register(nameof(SelectedLanguage), typeof(Language),
                typeof(LanguageSelectorControl), new PropertyMetadata(null));

        // —обытие, уведомл€ющее, что €зык изменЄн.
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
            // «десь можно вынести список поддерживаемых €зыков куда-нибудь (например, в ресурсы или в сервис локализации)
            var supportedLanguages = new List<Language>
            {
                new Language("ru-RU", "–усский"),
                new Language("en-US", "English")
            };

            LanguageComboBox.ItemsSource = supportedLanguages;

            // ќпредел€ем текущий €зык на основе ApplicationLanguages.PrimaryLanguageOverride
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
                // ≈сли включено автоматическое обновление (например, перезапуск), то выполн€ем его.
                if (AutoRestartOnLanguageChange)
                {
                    // «десь можно добавить подтверждение или логику, провер€ющую, можно ли сейчас перезапускать приложение.
                    Microsoft.Windows.AppLifecycle.AppInstance.Restart("");
                }
            }
        }
    }
}
