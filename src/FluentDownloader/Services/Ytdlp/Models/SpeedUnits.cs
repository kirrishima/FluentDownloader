using FluentDownloader.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FluentDownloader.Services.Ytdlp.Models;

[AttributeUsage(AttributeTargets.Field)]
public class LocalizedDisplayAttribute : Attribute
{
    public string ResourceKey { get; }

    public LocalizedDisplayAttribute(string resourceKey)
    {
        ResourceKey = resourceKey;
    }
}

public enum SpeedUnit
{
    [LocalizedDisplay("SpeedUnit_KilobytesPerSecond")]
    KilobytesPerSecond = 1024,

    [LocalizedDisplay("SpeedUnit_MegabytesPerSecond")]
    MegabytesPerSecond = KilobytesPerSecond * 1024,

    [LocalizedDisplay("SpeedUnit_GigabytesPerSecond")]
    GigabytesPerSecond = MegabytesPerSecond * 1024
}

public static class SpeedUnitExtensions
{
    /// <summary>
    /// Возвращает локализованное отображаемое имя единицы скорости, используя .resw файлы.
    /// </summary>
    public static string GetLocalizedDisplayName(this SpeedUnit unit)
    {
        // Получаем информацию о поле перечисления
        FieldInfo? field = unit.GetType().GetField(unit.ToString());
        if (field != null)
        {
            // Ищем атрибут с ключом ресурса
            var attribute = field.GetCustomAttribute<LocalizedDisplayAttribute>();
            if (attribute != null)
            {
                return LocalizedStrings.GetSpeedUnitsString(attribute.ResourceKey);
            }
        }
        // Если атрибут не найден или строка не определена, возвращаем имя элемента
        return unit.ToString();
    }

    /// <summary>
    /// Преобразует значение скорости в байты/сек, исходя из выбранной единицы.
    /// </summary>
    public static long ToBytes(this SpeedUnit unit, long value)
    {
        return value * (long)unit;
    }

    /// <summary>
    /// Преобразует значение скорости в байты/сек, исходя из выбранной единицы.
    /// </summary>
    public static long ToBytes(this SpeedUnit unit, double value)
    {
        return (long)(value * (long)unit);
    }

    /// <summary>
    /// Преобразует значение типа long (например, число байт) в соответствующую единицу скорости.
    /// Если число, разделённое на значение GigabytesPerSecond, дает целую часть больше 0, возвращается GigabytesPerSecond,
    /// иначе, если число, разделённое на значение MegabytesPerSecond, дает целую часть больше 0, возвращается MegabytesPerSecond,
    /// в остальных случаях – KilobytesPerSecond.
    /// </summary>
    /// <param name="value">Значение, например, количество байт.</param>
    /// <returns>Соответствующий элемент SpeedUnit.</returns>
    public static SpeedUnit ToSpeedUnit(this long value)
    {
        if (value / (long)SpeedUnit.GigabytesPerSecond > 0)
        {
            return SpeedUnit.GigabytesPerSecond;
        }
        else if (value / (long)SpeedUnit.MegabytesPerSecond > 0)
        {
            return SpeedUnit.MegabytesPerSecond;
        }
        else
        {
            return SpeedUnit.KilobytesPerSecond;
        }
    }
}
