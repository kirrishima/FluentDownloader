using FluentDownloader.Helpers;
using FluentDownloader.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using Windows.UI;

namespace FluentDownloader.Pages;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class StyleEditorPage : Page
{
    AcrylicBrush? AcrylicBrush { get; set; }

    private AccentColorPreview ColorPreview { get; set; } = null!;

    private bool _initialized = false;

    public StyleEditorPage()
    {
        this.InitializeComponent();
        this.NavigationCacheMode = NavigationCacheMode.Enabled;

        AcrylicBrush = StylesManager.Instance.WindowBackgroundBrush as AcrylicBrush;

        InitializeComponents();
        _initialized = true;
    }

    private void InitializeComponents()
    {
        var color = GetSavedAccentColorOrDefault();

        ColorPreview = new AccentColorPreview()
        {
            Text = color.ToString(),
            Background = new SolidColorBrush()
            {
                Color = color
            }
        };

        AccentColorPreview2.DataContext = ColorPreview;
        AccentColorPreview1.DataContext = ColorPreview;

        UseSystemAcentColorToggleSwitch.DataContext = App.AppSettings;

        switch (App.AppSettings.Appearance.BackdropMode)
        {
            case BackdropMode.Acrylic:
                AcrylicColorRadioButton.IsChecked = true;
                break;
            case BackdropMode.Mica:
                MicaRadioButton.IsChecked = true;
                break;
            case BackdropMode.Solid:
                SolidColorRadioButton.IsChecked = true;
                break;
            default:
                break;
        }
    }

    private void UpdateBrush(object? s, EventArgs args)
    {
        if (AcrylicBrush is null)
        {
            AcrylicBrush = new AcrylicBrush();
        }
        AcrylicBrush.TintColor = ColorSelectionService.Instance.SelectedColor;
        AcrylicBrush.Opacity = ColorSelectionService.Instance.Opacity / 100;
        AcrylicBrush.TintOpacity = ColorSelectionService.Instance.TintOpacity / 100;

        UseThemeButton_Click();

        ColorSelectionService.Instance.SaveClicked -= UpdateBrush;
    }

    private void UseThemeButton_Click()
    {

        StylesManager.Instance.WindowBackgroundBrush = AcrylicBrush;

        if (AcrylicBrush is not null)
        {
            App.AppSettings.Appearance.CustomAcrylicBrush = AcrylicBrush;
            App.AppSettings.Appearance.UseCustomAcrylicBrush = true;
        }
    }

    private void ResetToDefaultButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            App.AppSettings.Appearance.UseCustomAcrylicBrush = false;
            StylesManager.Instance.WindowBackgroundBrush = null;
            AcrylicColorRadioButton.IsChecked = true;
        }
        catch (Exception ex)
        {
            ExceptionHandler.HandleGeneralException(ex);
        }
    }

    private void ShowColorPickerButton_Click(object sender, RoutedEventArgs e)
    {
        ColorSelectionService.Instance.SaveClicked += UpdateBrush;
        ColorSelectionService.Instance.RequestShowColorPickerWithSliders();
    }

    private Color GetSavedAccentColorOrDefault()
    {
        return App.AppSettings.Appearance.AccentColor ?? App.SystemAccentColor;
    }

    private void UseSystemAcentColorToggleSwitch_Toggled(object sender, RoutedEventArgs e)
    {
        if (!UseSystemAcentColorToggleSwitch.IsOn)
        {
            App.AppSettings.Appearance.AccentColor = ColorPreview.Background?.Color;
            if (ColorPreview?.Background is not null)
            {
                ColorPreview.Background.Color = GetSavedAccentColorOrDefault();
            }
        }
        else
        {
            App.AppSettings.Appearance.AccentColor = null;
            if (ColorPreview?.Background is not null)
            {
                ColorPreview.Background.Color = App.SystemAccentColor;
            }
        }
    }

    private void SelectAccentColorButton_Click(object sender, RoutedEventArgs e)
    {
        ColorSelectionService.Instance.SaveClicked += SaveChosenAccentColor;
        ColorSelectionService.Instance.RequestShowColorPickerWithoutSliders();
    }

    private void SaveChosenAccentColor(object? s, EventArgs args)
    {
        ColorSelectionService.Instance.SaveClicked -= SaveChosenAccentColor;
        App.AppSettings.Appearance.AccentColor = ColorSelectionService.Instance.SelectedColor;

        if (ColorPreview is not null && ColorPreview.Background is not null)
        {
            ColorPreview.Background.Color = GetSavedAccentColorOrDefault();
        }
    }

    public class AccentColorPreview
    {
        public string? Text { get; set; }

        public SolidColorBrush? Background { get; set; }
    }

    private void MicaRadioButton_Checked(object sender, RoutedEventArgs e)
    {
        if (_initialized)
        {
            App.AppSettings.Appearance.BackdropMode = BackdropMode.Mica;

            if (StylesManager.Instance.WindowBackgroundBrush != null && StylesManager.Instance.WindowBackgroundBrush != AcrylicBrush)
            {
                StylesManager.Instance.WindowBackgroundBrush = null;
            }

            BackdropViewModel.Instance.EnableMica();
        }
    }

    private void AcrylicColorRadioButton_Checked(object sender, RoutedEventArgs e)
    {
        if (_initialized)
        {
            App.AppSettings.Appearance.BackdropMode = BackdropMode.Acrylic;

            if (StylesManager.Instance.WindowBackgroundBrush != null && StylesManager.Instance.WindowBackgroundBrush != AcrylicBrush)
            {
                StylesManager.Instance.WindowBackgroundBrush = null;
            }

            BackdropViewModel.Instance.EnableAcrylic();
        }
    }

    private void SolidColorRadioButton_Checked(object sender, RoutedEventArgs e)
    {
        if (_initialized)
        {
            App.AppSettings.Appearance.BackdropMode = BackdropMode.Solid;

            if (StylesManager.Instance.WindowBackgroundBrush == null)
            {
                StylesManager.Instance.WindowBackgroundBrush = StylesManager.GetDefaultBackground();
            }

            BackdropViewModel.Instance?.EnableSolid();
        }
    }
}
