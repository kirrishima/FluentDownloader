using FluentDownloader.ViewModels.Localization;

namespace FluentDownloader.ViewModels
{
    /// <summary>
    /// Комбинированная модель представления для MainPage, объединяющая общие свойства и локализацию.
    /// </summary>
    public class MainPageCombinedViewModel : BaseCombinedViewModel<MainPageLocalizationViewModel>
    {
        protected override void Initialize()
        {
            // При инициализации обновляем локализованные строки.
            Localization.UpdateLocalizedStrings();
        }
    }
}
