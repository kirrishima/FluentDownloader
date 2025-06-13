using FluentDownloader.Helpers;
using System.ComponentModel;

namespace FluentDownloader.ViewModels.Localization
{
    public class AppearancePageLocalizationViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        // Свойства, к которым будем привязывать элементы управления
        public string GoBackButtonText { get; set; } = null!;
        public string UseThemeButtonContent { get; set; } = null!;
        public string UseThemeButtonToolTip { get; set; } = null!;
        public string ResetStylesButtonContent { get; set; } = null!;
        public string ResetStylesButtonToolTip { get; set; } = null!;
        public string ResetToDefaultButtonContent { get; set; } = null!;
        public string ResetToDefaultButtonToolTip { get; set; } = null!;
        public string TintOpacitySliderHeader { get; set; } = null!;
        public string OpacitySliderHeader { get; set; } = null!;

        public AppearancePageLocalizationViewModel()
        {
            UpdateLocalizedStrings();
            LanguageChangedManager.LanguageChanged += (s, e) => UpdateLocalizedStrings();
        }

        public void UpdateLocalizedStrings()
        {
            GoBackButtonText = Helpers.LocalizedStrings.GetLeftSidebarString("GoBackButtonTextBlock/Text");
            UseThemeButtonContent = Helpers.LocalizedStrings.GetLeftSidebarString("UseThemeButton/Content");
            UseThemeButtonToolTip = Helpers.LocalizedStrings.GetLeftSidebarString("UseThemeButton/ToolTipService/ToolTip");
            ResetStylesButtonContent = Helpers.LocalizedStrings.GetLeftSidebarString("ResetStylesButton/Content");
            ResetStylesButtonToolTip = Helpers.LocalizedStrings.GetLeftSidebarString("ResetStylesButton/ToolTipService/ToolTip");
            ResetToDefaultButtonContent = Helpers.LocalizedStrings.GetLeftSidebarString("ResetToDefaultButton/Content");
            ResetToDefaultButtonToolTip = Helpers.LocalizedStrings.GetLeftSidebarString("ResetToDefaultButton/ToolTipService/ToolTip");
            TintOpacitySliderHeader = Helpers.LocalizedStrings.GetLeftSidebarString("TintOpacitySlider/Header");
            OpacitySliderHeader = Helpers.LocalizedStrings.GetLeftSidebarString("OpacitySlider/Header");
            OnPropertyChanged(string.Empty); // уведомляем, что все свойства изменились
        }

        protected void OnPropertyChanged(string propertyName) =>
             PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

}
