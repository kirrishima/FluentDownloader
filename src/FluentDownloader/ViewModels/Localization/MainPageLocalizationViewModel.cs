using FluentDownloader.Helpers;
using System.ComponentModel;

namespace FluentDownloader.ViewModels.Localization
{
    public class MainPageLocalizationViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        // Свойства для локализации элементов страницы MainPage
        public string VideoUrlLabelText { get; set; } = null!;
        public string UrlTextBoxPlaceholderText { get; set; } = null!;
        public string OpenEditUrlButtonContent { get; set; } = null!;
        public string OpenEditUrlButtonToolTip { get; set; } = null!;
        public string SaveToLabelText { get; set; } = null!;
        public string SavePathTextBoxPlaceholderText { get; set; } = null!;
        public string SavePathTextBoxToolTip { get; set; } = null!;
        public string SavePathTeachingTipTitle { get; set; } = null!;
        public string SavePathTeachingTipSubtitle { get; set; } = null!;
        public string SavePathButtonToolTip { get; set; } = null!;
        public string SavePathButtonText { get; set; } = null!;
        public string FormatTextBlockText { get; set; } = null!;
        public string FormatComboBoxHeader { get; set; } = null!;
        public string FormatComboBoxPlaceholderText { get; set; } = null!;
        public string FormatComboBoxToolTip { get; set; } = null!;
        public string FormatComboBoxItem1Content { get; set; } = null!;
        public string FormatComboBoxItem2Content { get; set; } = null!;
        public string FormatComboBoxItem3Content { get; set; } = null!;
        public string VideoFormatComboBoxHeader { get; set; } = null!;
        public string VideoFormatComboBoxPlaceholderText { get; set; } = null!;
        public string VideoFormatComboBoxToolTip { get; set; } = null!;
        public string AudioFormatComboBoxHeader { get; set; } = null!;
        public string AudioFormatComboBoxPlaceholderText { get; set; } = null!;
        public string AudioFormatComboBoxToolTip { get; set; } = null!;
        public string RecodeVideFormatComboBoxHeader { get; set; } = null!;
        public string RecodeVideFormatComboBoxPlaceholderText { get; set; } = null!;
        public string RecodeVideFormatComboBoxToolTip { get; set; } = null!;
        public string FormatTeachingTipTitle { get; set; } = null!;
        public string FormatTeachingTipSubtitle { get; set; } = null!;
        public string EditTextDialogTitle { get; set; } = null!;
        public string EditTextDialogCloseButtonText { get; set; } = null!;
        public string EditTextDialogPrimaryButtonText { get; set; } = null!;
        public string DownloadButtonToolTip { get; set; } = null!;
        public string DownloadButtonGetFormatsText { get; set; } = null!;
        public string DownloadButtonDownloadText { get; set; } = null!;
        public string DownloadButtonCancelPanelText { get; set; } = null!;
        public string ResetDownloadFieldsButtonText { get; set; } = null!;
        public string ResetDownloadFieldsButtonToolTip { get; set; } = null!;
        public string LogsBlockHeaderText { get; set; } = null!;

        public MainPageLocalizationViewModel()
        {
            UpdateLocalizedStrings();
            // Обновление локализованных строк при смене языка:
            LanguageChangedManager.LanguageChanged += (s, e) => UpdateLocalizedStrings();
        }

        public void UpdateLocalizedStrings()
        {
            // если это требуется внутренней логикой метода GetResourceString.
            VideoUrlLabelText = Helpers.LocalizedStrings.GetResourceString("VideoUrlLabel/Text");
            UrlTextBoxPlaceholderText = Helpers.LocalizedStrings.GetResourceString("UrlTextBox/PlaceholderText");
            OpenEditUrlButtonContent = Helpers.LocalizedStrings.GetResourceString("OpenEditUrlButton/Content");
            OpenEditUrlButtonToolTip = Helpers.LocalizedStrings.GetResourceString("OpenEditUrlButton/ToolTipService/ToolTip");
            SaveToLabelText = Helpers.LocalizedStrings.GetResourceString("SaveToLabel/Text");
            SavePathTextBoxPlaceholderText = Helpers.LocalizedStrings.GetResourceString("SavePathTextBox/PlaceholderText");
            SavePathTextBoxToolTip = Helpers.LocalizedStrings.GetResourceString("SavePathTextBox/ToolTipService/ToolTip");
            SavePathTeachingTipTitle = Helpers.LocalizedStrings.GetResourceString("SavePathTeachingTip/Title");
            SavePathTeachingTipSubtitle = Helpers.LocalizedStrings.GetResourceString("SavePathTeachingTip/Subtitle");
            SavePathButtonToolTip = Helpers.LocalizedStrings.GetResourceString("SavePathButton/ToolTipService/ToolTip");
            SavePathButtonText = Helpers.LocalizedStrings.GetResourceString("SavePathButtonTextBlock/Text");
            FormatTextBlockText = Helpers.LocalizedStrings.GetResourceString("FormatTextBlock/Text");
            FormatComboBoxHeader = Helpers.LocalizedStrings.GetResourceString("FormatComboBox/Header");
            FormatComboBoxPlaceholderText = Helpers.LocalizedStrings.GetResourceString("FormatComboBox/PlaceholderText");
            FormatComboBoxToolTip = Helpers.LocalizedStrings.GetResourceString("FormatComboBox/ToolTipService/ToolTip");
            FormatComboBoxItem1Content = Helpers.LocalizedStrings.GetResourceString("FormatComboBoxItem1/Content");
            FormatComboBoxItem2Content = Helpers.LocalizedStrings.GetResourceString("FormatComboBoxItem2/Content");
            FormatComboBoxItem3Content = Helpers.LocalizedStrings.GetResourceString("FormatComboBoxItem3/Content");
            VideoFormatComboBoxHeader = Helpers.LocalizedStrings.GetResourceString("VideoFormatComboBox/Header");
            VideoFormatComboBoxPlaceholderText = Helpers.LocalizedStrings.GetResourceString("VideoFormatComboBox/PlaceholderText");
            VideoFormatComboBoxToolTip = Helpers.LocalizedStrings.GetResourceString("VideoFormatComboBox/ToolTipService/ToolTip");
            AudioFormatComboBoxHeader = Helpers.LocalizedStrings.GetResourceString("AudioFormatComboBox/Header");
            AudioFormatComboBoxPlaceholderText = Helpers.LocalizedStrings.GetResourceString("AudioFormatComboBox/PlaceholderText");
            AudioFormatComboBoxToolTip = Helpers.LocalizedStrings.GetResourceString("AudioFormatComboBox/ToolTipService/ToolTip");
            RecodeVideFormatComboBoxHeader = Helpers.LocalizedStrings.GetResourceString("RecodeVideFormatComboBox/Header");
            RecodeVideFormatComboBoxPlaceholderText = Helpers.LocalizedStrings.GetResourceString("RecodeVideFormatComboBox/PlaceholderText");
            RecodeVideFormatComboBoxToolTip = Helpers.LocalizedStrings.GetResourceString("RecodeVideFormatComboBox/ToolTipService/ToolTip");
            FormatTeachingTipTitle = Helpers.LocalizedStrings.GetResourceString("FormatTeachingTip/Title");
            FormatTeachingTipSubtitle = Helpers.LocalizedStrings.GetResourceString("FormatTeachingTip/Subtitle");
            EditTextDialogTitle = Helpers.LocalizedStrings.GetResourceString("EditTextDialog/Title");
            EditTextDialogCloseButtonText = Helpers.LocalizedStrings.GetResourceString("EditTextDialog/CloseButtonText");
            EditTextDialogPrimaryButtonText = Helpers.LocalizedStrings.GetResourceString("EditTextDialog/PrimaryButtonText");
            DownloadButtonToolTip = Helpers.LocalizedStrings.GetResourceString("DownloadButton/ToolTipService/ToolTip");
            DownloadButtonGetFormatsText = Helpers.LocalizedStrings.GetResourceString("DownloadButtonGetFormatsTextBlock/Text");
            DownloadButtonDownloadText = Helpers.LocalizedStrings.GetResourceString("DownloadButtonDownloadTextBlock/Text");
            DownloadButtonCancelPanelText = Helpers.LocalizedStrings.GetResourceString("DownloadButtonCancelPanelTextBlock/Text");
            ResetDownloadFieldsButtonText = Helpers.LocalizedStrings.GetResourceString("ResetDownloadFieldsButtonText/Text");
            ResetDownloadFieldsButtonToolTip = Helpers.LocalizedStrings.GetResourceString("ResetDownloadFieldsButton/ToolTipService/ToolTip");
            LogsBlockHeaderText = Helpers.LocalizedStrings.GetResourceString("LogsBlockHeader/Text");


            OnPropertyChanged(string.Empty); // уведомление об изменении всех свойств
        }

        protected void OnPropertyChanged(string propertyName) =>
             PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
