using FluentDownloader.Helpers;
using FluentDownloader.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using System;

namespace FluentDownloader.Controls
{
    public sealed partial class ColorPickerControl : UserControl
    {
        public ColorPickerControl()
        {
            this.InitializeComponent();
            this.DataContext = ColorSelectionService.Instance;

            ColorSelectionService.Instance.ShowColorPickerRequested +=
                () =>
                {
                    // Перед показом устанавливаем начальные параметры
                    RootGrid.Visibility = Visibility.Visible;
                    var fadeInStoryboard = (Storyboard)this.Resources["FadeInStoryboard"];
                    fadeInStoryboard.Begin();
                };

            var brush = StylesManager.Instance.WindowBackgroundBrush as AcrylicBrush;
            if (brush != null)
            {
                // Если в AcrylicBrush TintColor имеет смысл, либо преобразуйте его в тип Color
                ColorSelectionService.Instance.SelectedColor = brush.TintColor;
                ColorSelectionService.Instance.Opacity = (int)(brush.Opacity * 100);
                ColorSelectionService.Instance.TintOpacity = (int)(brush.TintOpacity * 100);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Получаем Storyboard из ресурсов
            var fadeOutStoryboard = (Storyboard)this.Resources["FadeOutStoryboard"];

            // Подписываемся на событие завершения анимации
            fadeOutStoryboard.Completed += FadeOutStoryboard_Completed;

            // Запускаем анимацию
            fadeOutStoryboard.Begin();
        }

        private void FadeOutStoryboard_Completed(object? sender, object e)
        {
            // Отписываемся от события, чтобы не было множественных подписок
            var storyboard = sender as Storyboard;

            if (storyboard != null)
            {
                storyboard.Completed -= FadeOutStoryboard_Completed;
            }
            // Сбрасываем прозрачность для будущего использования и скрываем элемент
            RootGrid.Visibility = Visibility.Collapsed;

            // Вызываем сохранение настроек
            ColorSelectionService.Instance.SendSaveButtonClicked(this, EventArgs.Empty);
        }
    }
}
