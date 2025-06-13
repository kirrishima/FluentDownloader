using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentDownloader.Converters
{
    internal class BackDropToToggleSwitchState : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, string language)
        {
            if (App.MainWindow.SystemBackdrop is DesktopAcrylicBackdrop)
            {
                return true;
            }
            else if (App.MainWindow.SystemBackdrop is MicaBackdrop)
            {
                return false;
            }
            else
            {
                throw new ArgumentException($"{nameof(value)} has inccorect data type {value.GetType().FullName}. " +
                    $"Expected: {typeof(DesktopAcrylicBackdrop).FullName} or {typeof(MicaBackdrop).FullName}");
            }
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
