using FluentDownloader.ViewModels.Localization;

namespace FluentDownloader.ViewModels
{
    /// <summary>
    /// Комбинированная модель представления для страницы внешнего вида, объединяющая локализацию и общие свойства.
    /// </summary>
    public class AppearancePageCombinedViewModel : BaseCombinedViewModel<AppearancePageLocalizationViewModel>
    {
        protected override void Initialize()
        {
            // Можно выполнить дополнительную инициализацию, если требуется.
            // Например, сразу обновить локализованные строки:
            Localization.UpdateLocalizedStrings();
        }
    }
}
