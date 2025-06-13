using FluentDownloader.ViewModels;
using Microsoft.UI.Xaml.Controls;

namespace FluentDownloader.Controls
{
    public sealed partial class ThemeChangeButton : UserControl
    {
        public ThemeChangeButton()
        {
            this.InitializeComponent();
            DataContext = ThemeViewModel.Instance;
        }
    }
}
