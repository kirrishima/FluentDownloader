using FluentDownloader.ViewModels;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace FluentDownloader.Models
{
    public class ColorJsonModel
    {
        public Color? BrushColor { get; set; } = null;

        public double Opacity { get; set; } = 1;

        public double TintOpacity { get; set; } = 1;
    }

    public class ColorSettingsJson
    {
        public ElementTheme Theme { get; set; } = ElementTheme.Default;

        public Color? AccentColor { get; set; } = null;

        public ColorJsonModel ColorJsonModel { get; set; } = new ColorJsonModel();

        public BackdropMode BackdropMode { get; set; } = BackdropMode.Acrylic;

        public Language Language { get; set; } = new Language();
    }
}
