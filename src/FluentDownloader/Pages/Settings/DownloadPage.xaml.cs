using FluentDownloader.Extensions;
using FluentDownloader.Helpers;
using FluentDownloader.Services.Ytdlp.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace FluentDownloader.Pages.Settings;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class DownloadPage : Page
{
    private SpeedUnit _currentSpeedUnit = SpeedUnit.KilobytesPerSecond;

    public DownloadPage()
    {
        this.InitializeComponent();
        //RateLimitComboBox.PopulateComboBoxWithEnum(SpeedUnit.KilobytesPerSecond);

        RateLimitNumberBox.Maximum = long.MaxValue / (long)SpeedUnit.GigabytesPerSecond;

        long selected = App.AppSettings.Download.RateLimitInBytes;

        SpeedUnit? speedUnit = null;
        if (selected > 0)
        {
            speedUnit = selected.ToSpeedUnit();
        }

        RateLimitComboBox.Items.Clear();
        RateLimitComboBox.Items.Add(SpeedUnit.KilobytesPerSecond.GetLocalizedDisplayName());
        RateLimitComboBox.SelectedIndex = RateLimitComboBox.Items.Count - 1;
        if (speedUnit == SpeedUnit.KilobytesPerSecond)
        {
            RateLimitComboBox.SelectedIndex = RateLimitComboBox.Items.Count - 1;
            _currentSpeedUnit = SpeedUnit.KilobytesPerSecond;
        }

        RateLimitComboBox.Items.Add(SpeedUnit.MegabytesPerSecond.GetLocalizedDisplayName());

        if (speedUnit == SpeedUnit.MegabytesPerSecond)
        {
            RateLimitComboBox.SelectedIndex = RateLimitComboBox.Items.Count - 1;
            _currentSpeedUnit = SpeedUnit.MegabytesPerSecond;
        }

        RateLimitComboBox.Items.Add(SpeedUnit.GigabytesPerSecond.GetLocalizedDisplayName());

        if (speedUnit == SpeedUnit.GigabytesPerSecond)
        {
            RateLimitComboBox.SelectedIndex = RateLimitComboBox.Items.Count - 1;
            _currentSpeedUnit = SpeedUnit.GigabytesPerSecond;
        }

        double initialValue = Math.Round((double)App.AppSettings.Download.RateLimitInBytes / (double)_currentSpeedUnit, 2);
        RateLimitNumberBox.Value = initialValue;
        RateLimitComboBox.SelectionChanged += RateLimitComboBox_SelectionChanged;
    }

    private void NavigateToSaveTemplatePage(object sender, RoutedEventArgs e)
    {
        var parentPage = this.FindParent<SettingsPage>();
        if (parentPage is not null)
        {
            parentPage.NavigateTo(
                typeof(SaveTemlatePage),
                LocalizedStrings.GetSettingsString("InstallationSettings/Text"),
                null
                );
        }
    }

    /// <summary>
    /// ���������� �������, ������� ������������� ������������ �������� � NumberBox ��� ����� ������� ���������.
    /// </summary>
    private void RateLimitComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        // ���������� ����� ������� ��������� �� ������� ComboBox.
        SpeedUnit newUnit = RateLimitComboBox.SelectedIndex switch
        {
            0 => SpeedUnit.KilobytesPerSecond,
            1 => SpeedUnit.MegabytesPerSecond,
            2 => SpeedUnit.GigabytesPerSecond,
            _ => SpeedUnit.KilobytesPerSecond
        };

        // �������� ������� �������� �� NumberBox (� ������ �������) ��� double.
        double oldValue = RateLimitNumberBox.Value;

        // ��������� �������� � ����� � �������������� ������� �������.
        double bytes = oldValue * (double)_currentSpeedUnit;

        // ��������� �������� ��� ����� ������� � ��������� �� ���� ������.
        double newValue = Math.Round(bytes / (double)newUnit, 2);

        // ���� ����������� � ��������� ������� �������� � �������� �������� (��� ���, ��� � ������ �������� �������������),
        // ��������� ������ �������� � ��������������� ���������� ����� � ComboBox.
        if (newValue == 0 && bytes > 0)
        {
            // �������� ��������� ����������, ����� �� ������� ����������� ������� ���������.
            RateLimitComboBox.SelectionChanged -= RateLimitComboBox_SelectionChanged;

            // ��������������� ���������� �����.
            int oldIndex = _currentSpeedUnit switch
            {
                SpeedUnit.KilobytesPerSecond => 0,
                SpeedUnit.MegabytesPerSecond => 1,
                SpeedUnit.GigabytesPerSecond => 2,
                _ => 0
            };
            RateLimitComboBox.SelectedIndex = oldIndex;

            // �������� ���������� ����������.
            RateLimitComboBox.SelectionChanged += RateLimitComboBox_SelectionChanged;
            // ����� ����� ��������� ������������, ��� �������� ������� ���� ��� ��������� �������.
        }
        else
        {
            // ���� ��������� ����������, ��������� NumberBox � ��������� ����� �������.
            RateLimitNumberBox.Value = newValue;
            _currentSpeedUnit = newUnit;
        }
    }

    private void SaveRateLimitButton_Click(object sender, RoutedEventArgs e)
    {
        // �������� �������� �������� �� NumberBox
        var numericValue = RateLimitNumberBox.Value;

        // ���������� ��������� ������� ��������� �� ������� ComboBox
        SpeedUnit unit = RateLimitComboBox.SelectedIndex switch
        {
            0 => SpeedUnit.KilobytesPerSecond,
            1 => SpeedUnit.MegabytesPerSecond,
            2 => SpeedUnit.GigabytesPerSecond,
            _ => SpeedUnit.KilobytesPerSecond
        };

        // ����������� ��������� �������� � �����
        long rateLimitInBytes = unit.ToBytes(numericValue);

        // ������: ��������� ��������� � ���������� ����������
        App.AppSettings.Download.RateLimitInBytes = rateLimitInBytes;
    }
}
