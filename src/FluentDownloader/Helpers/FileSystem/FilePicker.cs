using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Pickers;
using Windows.Storage;
using WinRT.Interop;

namespace FluentDownloader.Helpers.FileSystem;

public static class FilePicker
{
    /// <summary>
    /// Открывает диалог выбора файла с заданными расширениями.
    /// </summary>
    /// <param name="window">Окно, с которым будет связан диалог выбора.</param>
    /// <param name="allowedFileExtensions">Коллекция разрешённых расширений (например, ".txt", ".jpg").</param>
    /// <returns>Выбранный StorageFile или null, если выбор отменён.</returns>
    public static async Task<StorageFile> PickFileAsync(Window window, IEnumerable<string> allowedFileExtensions)
    {
        var picker = new FileOpenPicker();
        // Получаем дескриптор окна для привязки диалога
        IntPtr hWnd = WindowNative.GetWindowHandle(window);
        InitializeWithWindow.Initialize(picker, hWnd);

        // Задаём фильтр по расширениям
        picker.FileTypeFilter.Clear();
        foreach (var ext in allowedFileExtensions)
        {
            picker.FileTypeFilter.Add(ext);
        }

        return await picker.PickSingleFileAsync();
    }

    /// <summary>
    /// Открывает диалог сохранения файла.
    /// </summary>
    /// <param name="window">Окно, с которым будет связан диалог сохранения.</param>
    /// <param name="defaultFileExtension">Расширение файла по умолчанию (например, ".txt").</param>
    /// <param name="suggestedFileName">Предлагаемое имя файла.</param>
    /// <param name="fileTypeChoices">Словарь, где ключ – описание типа файла, а значение – список расширений (например, { "Текстовый документ", new List&lt;string&gt; { ".txt" } }).</param>
    /// <returns>Выбранный StorageFile для сохранения или null, если выбор отменён.</returns>
    public static async Task<StorageFile> SaveFileAsync(
        Window window,
        string defaultFileExtension,
        string suggestedFileName,
        IDictionary<string, List<string>> fileTypeChoices)
    {
        var savePicker = new FileSavePicker();
        IntPtr hWnd = WindowNative.GetWindowHandle(window);
        InitializeWithWindow.Initialize(savePicker, hWnd);

        savePicker.DefaultFileExtension = defaultFileExtension;
        savePicker.SuggestedFileName = suggestedFileName;

        savePicker.FileTypeChoices.Clear();
        foreach (var kvp in fileTypeChoices)
        {
            savePicker.FileTypeChoices.Add(kvp.Key, kvp.Value);
        }

        return await savePicker.PickSaveFileAsync();
    }

    /// <summary>
    /// Открывает диалог выбора файла с фильтром на расширение .exe.
    /// </summary>
    /// <param name="window">Окно, с которым будет связан диалог выбора файла.</param>
    /// <returns>Выбранный файл (.exe) или null, если выбор отменён.</returns>
    public static async Task<StorageFile> PickExeFileAsync(Window window)
    {
        var picker = new FileOpenPicker();

        // Получаем дескриптор окна и инициализируем диалог выбора файла
        IntPtr hWnd = WindowNative.GetWindowHandle(window);
        InitializeWithWindow.Initialize(picker, hWnd);

        // Очистка фильтра и добавление разрешённого расширения .exe
        picker.FileTypeFilter.Clear();
        picker.FileTypeFilter.Add(".exe");

        // Открываем диалог и возвращаем выбранный файл
        return await picker.PickSingleFileAsync();
    }
}
