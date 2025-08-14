using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;
using System.Globalization;

namespace FluentDownloader.Converters
{
    public class IntegerToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Конвертирует числовое значение в Visibility.
        /// По умолчанию: visible, если value >= threshold (threshold = 1).
        /// Параметр (converterParameter) поддерживает формат:
        ///   "Invert"                      -> инвертировать результат
        ///   "Invert=true"                 -> то же самое
        ///   "Threshold=2" или "2"         -> задать порог
        ///   "Threshold=2;Invert=true"     -> комбинация
        /// Разделители: ';' или ','.
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            // Попытка получить число из value
            if (!TryGetDouble(value, out double number))
            {
                // Если не число — скрываем элемент
                return Visibility.Collapsed;
            }

            // Парсим parameter
            bool invert = false;
            double threshold = 1.0;

            if (parameter != null)
            {
                ParseParameter(parameter.ToString() ?? string.Empty, ref threshold, ref invert);
            }

            bool visible = number >= threshold;
            if (invert) visible = !visible;

            return visible ? Visibility.Visible : Visibility.Collapsed;
        }

        public object? ConvertBack(object value, Type targetType, object parameter, string language)
        {
            // Как правило не используется. Можно бросить исключение или вернуть DependencyProperty.UnsetValue.
            throw new NotSupportedException();
        }

        private static bool TryGetDouble(object value, out double result)
        {
            result = 0;
            if (value == null) return false;

            switch (value)
            {
                case double d:
                    result = d; return true;
                case float f:
                    result = f; return true;
                case int i:
                    result = i; return true;
                case long l:
                    result = l; return true;
                case short s:
                    result = s; return true;
                case byte b:
                    result = b; return true;
                case decimal dec:
                    result = (double)dec; return true;
                case string str:
                    return double.TryParse(str, NumberStyles.Any, CultureInfo.InvariantCulture, out result)
                        || double.TryParse(str, NumberStyles.Any, CultureInfo.CurrentCulture, out result);
                default:
                    // Попытка через IConvertible
                    if (value is IConvertible conv)
                    {
                        try
                        {
                            result = conv.ToDouble(CultureInfo.InvariantCulture);
                            return true;
                        }
                        catch { }
                    }
                    return false;
            }
        }

        private static void ParseParameter(string param, ref double threshold, ref bool invert)
        {
            if (string.IsNullOrWhiteSpace(param)) return;

            // Разделим на части по ; или ,
            var parts = param.Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var raw in parts)
            {
                var part = raw.Trim();

                // Если просто "Invert" или "True"/"False"
                if (part.Equals("Invert", StringComparison.OrdinalIgnoreCase) ||
                    part.Equals("True", StringComparison.OrdinalIgnoreCase) ||
                    part.Equals("False", StringComparison.OrdinalIgnoreCase))
                {
                    if (part.Equals("Invert", StringComparison.OrdinalIgnoreCase) || part.Equals("True", StringComparison.OrdinalIgnoreCase))
                        invert = true;
                    else
                        invert = false;
                    continue;
                }

                // Попробуем разобрать как key=value
                var kv = part.Split(new[] { '=' }, 2);
                if (kv.Length == 1)
                {
                    // Если это просто число -> порог
                    if (double.TryParse(kv[0], NumberStyles.Any, CultureInfo.InvariantCulture, out var v1) ||
                        double.TryParse(kv[0], NumberStyles.Any, CultureInfo.CurrentCulture, out v1))
                    {
                        threshold = v1;
                    }
                    continue;
                }

                var key = kv[0].Trim();
                var val = kv[1].Trim();

                if (key.Equals("Threshold", StringComparison.OrdinalIgnoreCase) ||
                    key.Equals("T", StringComparison.OrdinalIgnoreCase))
                {
                    if (double.TryParse(val, NumberStyles.Any, CultureInfo.InvariantCulture, out var v) ||
                        double.TryParse(val, NumberStyles.Any, CultureInfo.CurrentCulture, out v))
                    {
                        threshold = v;
                    }
                }
                else if (key.Equals("Invert", StringComparison.OrdinalIgnoreCase))
                {
                    if (bool.TryParse(val, out var b)) invert = b;
                }
            }
        }
    }
}
