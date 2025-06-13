using FluentDownloader.Helpers;
using Microsoft.UI.Xaml.Media;
using System.ComponentModel;

namespace FluentDownloader.ViewModels.Localization
{
    /// <summary>
    /// Унифицированная базовая модель представления, объединяющая общие свойства (например, фон окна)
    /// и локализацию, реализуемую в TLocalization. В настоящей реализации не используется, 
    /// так как локализация реализованна через статические ресурсы
    /// </summary>
    /// <typeparam name="TLocalization">Тип модели локализации, который должен реализовывать INotifyPropertyChanged и иметь конструктор по умолчанию.</typeparam>
    public abstract class BaseCombinedViewModel<TLocalization> : INotifyPropertyChanged
        where TLocalization : INotifyPropertyChanged, new()
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Модель локализации для страницы.
        /// </summary>
        public TLocalization Localization { get; } = new TLocalization();

        /// <summary>
        /// Фоновая кисть для окна, получаемая из StylesManager.
        /// </summary>
        public Brush? WindowBackgroundBrush => StylesManager.Instance.WindowBackgroundBrush;

        protected BaseCombinedViewModel()
        {
            // Подписка на изменение фоновой кисти
            StylesManager.Instance.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(StylesManager.Instance.WindowBackgroundBrush))
                {
                    OnPropertyChanged(nameof(WindowBackgroundBrush));
                }
            };

            // При изменениях в локализации можно пробрасывать уведомление (если требуется обновлять привязки)
            Localization.PropertyChanged += (s, e) =>
            {
                OnPropertyChanged(e.PropertyName ?? string.Empty);
            };

            Initialize();
        }

        /// <summary>
        /// Метод для дополнительной инициализации в наследниках.
        /// </summary>
        protected virtual void Initialize() { }

        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
