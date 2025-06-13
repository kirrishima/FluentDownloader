using CommunityToolkit.WinUI.Helpers;
using FluentDownloader.Helpers;
using FluentDownloader.Settings;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text.Json;

namespace FluentDownloader.Pages;

public sealed partial class SettingsPage : Page
{
    public ObservableCollection<BreadcrumbItem> BreadcrumbItems { get; } = new ObservableCollection<BreadcrumbItem>();

    public SettingsPage()
    {
        this.InitializeComponent();

        Breadcrumb.ItemsSource = BreadcrumbItems;

        ContentFrame.Navigate(typeof(Settings.HomePage));
        BreadcrumbItems.Add(new BreadcrumbItem
        {
            DisplayName = LocalizedStrings.GetSettingsString("HomePageHeader"),
            PageType = typeof(Settings.HomePage)
        });
    }

    /// <summary>
    /// ���������� ��� ��������� ������ Frame. ����� ����� ��������� ������� ������, ���� ���������.
    /// ��������, ����� ������� �������� �������� ��� ��������� �����.
    /// </summary>
    private void ContentFrame_Navigated(object sender, NavigationEventArgs e)
    {
    }

    /// <summary>
    /// ���������� ������� �� ������� BreadcrumbBar.
    /// ��� ������� ��������� �� ��������� �������� � ������� �� ��������� ������� ������.
    /// </summary>
    private void Breadcrumb_ItemClick(BreadcrumbBar sender, BreadcrumbBarItemClickedEventArgs e)
    {
        if (e.Item is BreadcrumbItem item)
        {
            int index = BreadcrumbItems.IndexOf(item);

            while (BreadcrumbItems.Count - 1 > index)
            {
                BreadcrumbItems.RemoveAt(BreadcrumbItems.Count - 1);
            }

            if (ContentFrame.CurrentSourcePageType != item.PageType)
            {
                ContentFrame.Navigate(item.PageType);
            }
        }
    }


    /// <summary>
    /// ����� ��� ��������� �� ����� �����������.
    /// ��������� ��� �� �������� ������� (��������, HomePage), ����� �������� ����� ������ � ������� �� ������ ��������.
    /// </summary>
    /// <param name="pageType">��� ��������, �� ������� ���������</param>
    /// <param name="displayName">������������ ��� ��� ������� ������</param>
    /// <param name="icon">���������� ������������� (������)</param>
    public void NavigateTo(Type pageType, string displayName, string? icon)
    {
        BreadcrumbItems.Add(new BreadcrumbItem
        {
            DisplayName = displayName,
            Icon = icon,
            PageType = pageType
        });

        ContentFrame.Navigate(pageType);
    }

    private void GoBackButton_Click(object sender, RoutedEventArgs e)
    {
        if (BreadcrumbItems.Count > 1)
        {
            BreadcrumbItems.RemoveAt(BreadcrumbItems.Count - 1);
            var target = BreadcrumbItems[^1];
            ContentFrame.Navigate(target.PageType);
        }
        else
        {
            App.EventAggregator.PageGoBack();
        }
    }
}

public class BreadcrumbItem
{
    /// <summary>
    /// ������������ ��� ��� �������� (��������, "Home", "SubPage1" � �.�.).
    /// </summary>
    public string? DisplayName { get; set; }

    /// <summary>
    /// ���������� ������������� (������). ����� ������ Unicode-������, ��� � FontIcon.
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// ��� �������� ��� ���������.
    /// </summary>
    public Type? PageType { get; set; }
}

public static class VisualTreeHelperExtensions
{
    public static T? FindParent<T>(this DependencyObject child) where T : DependencyObject
    {
        DependencyObject parentObject = VisualTreeHelper.GetParent(child);
        if (parentObject == null)
            return null;

        if (parentObject is T parent)
            return parent;

        return FindParent<T>(parentObject);
    }
}
