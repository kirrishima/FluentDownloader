using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml.Media;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FluentDownloader.ViewModels
{
    /// <summary>
    /// Режим заднего фона.
    /// </summary>
    public enum BackdropMode
    {
        Acrylic, // Эффект акрила
        Mica,    // Эффект слюды (mica)
        Solid    // Сплошной цвет (нет эффекта)
    }

    /// <summary>
    /// ViewModel для управления эффектом заднего фона.
    /// Реализует поддержку трех режимов: акрил, слюда и сплошной цвет.
    /// </summary>
    public class BackdropViewModel : INotifyPropertyChanged
    {
        private static readonly BackdropViewModel _instance = new();
        public static BackdropViewModel Instance => _instance;

        // Создаем экземпляры эффектов для акрила и слюды.
        private readonly DesktopAcrylicBackdrop _acrylicBackdrop = new();
        private readonly MicaBackdrop _micaBackdrop = new();

        private BackdropMode _currentBackdropMode = App.AppSettings.Appearance.BackdropMode;
        /// <summary>
        /// Текущий выбранный режим заднего фона.
        /// При установке обновляется свойство SystemBackdrop.
        /// </summary>
        public BackdropMode CurrentBackdropMode
        {
            get => _currentBackdropMode;
            set
            {
                if (_currentBackdropMode != value)
                {
                    _currentBackdropMode = value;
                    UpdateSystemBackdrop();
                    OnPropertyChanged();
                }
            }
        }

        private SystemBackdrop? _systemBackdrop;
        /// <summary>
        /// Свойство, которое привязывается к эффекту заднего фона.
        /// Для режима Solid устанавливается в null.
        /// </summary>
        public SystemBackdrop? SystemBackdrop
        {
            get => _systemBackdrop;
            private set
            {
                if (_systemBackdrop != value)
                {
                    _systemBackdrop = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Обновляет SystemBackdrop в зависимости от выбранного режима.
        /// </summary>
        public void UpdateSystemBackdrop()
        {
            switch (CurrentBackdropMode)
            {
                case BackdropMode.Acrylic:
                    SystemBackdrop = _acrylicBackdrop;
                    break;
                case BackdropMode.Mica:
                    SystemBackdrop = _micaBackdrop;
                    break;
                case BackdropMode.Solid:
                default:
                    SystemBackdrop = null;
                    break;
            }
        }

        // Методы для установки режима заднего фона

        /// <summary>
        /// Включает акриловый эффект.
        /// </summary>
        public void EnableAcrylic() => CurrentBackdropMode = BackdropMode.Acrylic;

        /// <summary>
        /// Включает эффект слюды.
        /// </summary>
        public void EnableMica() => CurrentBackdropMode = BackdropMode.Mica;

        /// <summary>
        /// Устанавливает сплошной фон (без эффекта).
        /// </summary>
        public void EnableSolid() => CurrentBackdropMode = BackdropMode.Solid;

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
