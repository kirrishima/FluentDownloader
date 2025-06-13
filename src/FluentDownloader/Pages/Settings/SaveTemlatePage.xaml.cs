using FluentDownloader.Helpers;
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

namespace FluentDownloader.Pages.Settings
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SaveTemlatePage : Page
    {
        public SaveTemlatePage()
        {
            this.InitializeComponent();
            TemplateTextBox.Text = App.AppSettings.Download.FileOutputTemplate;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            TemplateTextBox.Text = TemplateTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(TemplateTextBox.Text))
            {
                SaveButtonTeachingTip.Title = LocalizedStrings.GetSettingsString("SaveButtonTeachingTipEmpty");
                TeachingTipHelper.Show(SaveButtonTeachingTip, TimeSpan.FromSeconds(5), DispatcherQueue);
                return;
            }
            else if (!TemplateTextBox.Text.EndsWith(".%(ext)s"))
            {
                SaveButtonTeachingTip.Title = LocalizedStrings.GetSettingsString("SaveButtonTeachingTipWrong");
                TeachingTipHelper.Show(SaveButtonTeachingTip, TimeSpan.FromSeconds(5), DispatcherQueue);
                return;
            }
            else
            {
                App.AppSettings.Download.FileOutputTemplate = TemplateTextBox.Text;
            }
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            TemplateTextBox.Text = App.AppSettings.Download.FileOutputTemplate;
        }

        private void RestoreDefaultButton_Click(object sender, RoutedEventArgs e)
        {
            App.AppSettings.Download.RestoreDefaultFileOutputTemplate();
            TemplateTextBox.Text = App.AppSettings.Download.FileOutputTemplate;
        }
    }
}
