using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;
using System.Globalization;

namespace FluentDownloader.Converters
{
    /// <summary>
    /// Преобразует объект в Visibility:
    /// - null или пустая строка => false => Collapsed
    /// - bool => его значение
    /// - строка "true"/"false" => соответствующее значение
    /// - число: 0 => false, иначе true
    /// - другие объекты => true
    /// Параметр: "Invert" или "Invert=true" для инверсии результата.
    /// </summary>
    public class ObjectToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool invert = false;
            double number;
            bool result;

            // парсим параметр
            if (parameter is string p && !string.IsNullOrWhiteSpace(p))
            {
                var parts = p.Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var part in parts)
                {
                    var t = part.Trim();
                    if (t.Equals("Invert", StringComparison.OrdinalIgnoreCase)
                        || t.Equals("True", StringComparison.OrdinalIgnoreCase)
                        || t.Equals("Invert=true", StringComparison.OrdinalIgnoreCase)
                        || t.Equals("Invert:True", StringComparison.OrdinalIgnoreCase))
                    {
                        invert = true;
                    }
                    else if (t.StartsWith("Invert=", StringComparison.OrdinalIgnoreCase))
                    {
                        var v = t.Substring(t.IndexOf('=') + 1);
                        if (bool.TryParse(v, out var bv)) invert = bv;
                    }
                }
            }

            // null -> false
            if (value == null)
            {
                result = false;
            }
            else if (value is bool b)
            {
                result = b;
            }
            else if (value is string s)
            {
                if (string.IsNullOrWhiteSpace(s))
                {
                    result = false;
                }
                else if (bool.TryParse(s, out var parsedBool))
                {
                    result = parsedBool;
                }
                else if (double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out number)
                         || double.TryParse(s, NumberStyles.Any, CultureInfo.CurrentCulture, out number))
                {
                    result = Math.Abs(number) > double.Epsilon;
                }
                else
                {
                    // ненулевая строка без parse -> считаем true (например "something")
                    result = true;
                }
            }
            else if (IsNumber(value, out number))
            {
                result = Math.Abs(number) > double.Epsilon;
            }
            else
            {
                // для всех прочих ссылочных объектов — считаем true
                result = true;
            }

            if (invert) result = !result;
            return result ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            // Обычно не нужен
            throw new NotSupportedException();
        }

        private static bool IsNumber(object value, out double number)
        {
            number = 0;
            switch (value)
            {
                case byte bb: number = bb; return true;
                case sbyte sb: number = sb; return true;
                case short sh: number = sh; return true;
                case ushort ush: number = ush; return true;
                case int i: number = i; return true;
                case uint ui: number = ui; return true;
                case long l: number = l; return true;
                case ulong ul: number = ul; return true;
                case float f: number = f; return true;
                case double d: number = d; return true;
                case decimal dec: number = (double)dec; return true;
                default:
                    if (value is IConvertible conv)
                    {
                        try
                        {
                            number = conv.ToDouble(CultureInfo.InvariantCulture);
                            return true;
                        }
                        catch { }
                    }
                    return false;
            }
        }
    }
}
