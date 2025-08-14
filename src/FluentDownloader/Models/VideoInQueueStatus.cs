using FluentDownloader.Helpers;
using FluentDownloader.Models;
using FluentDownloader.Services.Ytdlp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FluentDownloader.Models;

public enum VideoInQueueStatus
{
    [LocalizedDisplay("VideoInQueueStatus_Success")]
    Success,

    [LocalizedDisplay("VideoInQueueStatus_InQueue")]
    InQueue,

    [LocalizedDisplay("VideoInQueueStatus_Failed")]
    Failed,

    [LocalizedDisplay("VideoInQueueStatus_Downloading")]
    Downloading
}

public static class VideoInQueueStatusExtension
{
    public static string GetLocalizedDisplayName(this VideoInQueueStatus unit)
    {
        // Получаем информацию о поле перечисления
        FieldInfo? field = unit.GetType().GetField(unit.ToString());
        if (field != null)
        {
            // Ищем атрибут с ключом ресурса
            var attribute = field.GetCustomAttribute<LocalizedDisplayAttribute>();
            if (attribute != null)
            {
                return LocalizedStrings.GetResourceString(attribute.ResourceKey);
            }
        }
        // Если атрибут не найден или строка не определена, возвращаем имя элемента
        return unit.ToString();
    }
}