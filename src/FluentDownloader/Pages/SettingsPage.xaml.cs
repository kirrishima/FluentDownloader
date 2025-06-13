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
    /// Вызывается при навигации внутри Frame. Здесь можно обновлять хлебные крошки, если требуется.
    /// Например, можно очищать «будущие» элементы при навигации назад.
    /// </summary>
    private void ContentFrame_Navigated(object sender, NavigationEventArgs e)
    {
    }

    /// <summary>
    /// Обработчик нажатия на элемент BreadcrumbBar.
    /// При нажатии переходим на выбранную страницу и удаляем из коллекции «лишние» крошки.
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
    /// Метод для навигации на новую подстраницу.
    /// Вызывайте его из дочерних страниц (например, HomePage), чтобы добавить новую крошку и перейти на нужную страницу.
    /// </summary>
    /// <param name="pageType">Тип страницы, на которую переходим</param>
    /// <param name="displayName">Отображаемое имя для хлебной крошки</param>
    /// <param name="icon">Символьное представление (иконка)</param>
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
    /// Отображаемое имя для элемента (например, "Home", "SubPage1" и т.д.).
    /// </summary>
    public string? DisplayName { get; set; }

    /// <summary>
    /// Символьное представление (иконка). Можно задать Unicode-символ, как в FontIcon.
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// Тип страницы для навигации.
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
