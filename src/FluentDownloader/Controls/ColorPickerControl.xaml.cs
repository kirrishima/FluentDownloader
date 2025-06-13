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
                    // ����� ������� ������������� ��������� ���������
                    RootGrid.Visibility = Visibility.Visible;
                    var fadeInStoryboard = (Storyboard)this.Resources["FadeInStoryboard"];
                    fadeInStoryboard.Begin();
                };

            var brush = StylesManager.Instance.WindowBackgroundBrush as AcrylicBrush;
            if (brush != null)
            {
                // ���� � AcrylicBrush TintColor ����� �����, ���� ������������ ��� � ��� Color
                ColorSelectionService.Instance.SelectedColor = brush.TintColor;
                ColorSelectionService.Instance.Opacity = (int)(brush.Opacity * 100);
                ColorSelectionService.Instance.TintOpacity = (int)(brush.TintOpacity * 100);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // �������� Storyboard �� ��������
            var fadeOutStoryboard = (Storyboard)this.Resources["FadeOutStoryboard"];

            // ������������� �� ������� ���������� ��������
            fadeOutStoryboard.Completed += FadeOutStoryboard_Completed;

            // ��������� ��������
            fadeOutStoryboard.Begin();
        }

        private void FadeOutStoryboard_Completed(object? sender, object e)
        {
            // ������������ �� �������, ����� �� ���� ������������� ��������
            var storyboard = sender as Storyboard;

            if (storyboard != null)
            {
                storyboard.Completed -= FadeOutStoryboard_Completed;
            }
            // ���������� ������������ ��� �������� ������������� � �������� �������
            RootGrid.Visibility = Visibility.Collapsed;

            // �������� ���������� ��������
            ColorSelectionService.Instance.SendSaveButtonClicked(this, EventArgs.Empty);
        }
    }
}
