using Microsoft.UI.Xaml;
using System;
using System.Runtime.InteropServices;

namespace FluentDownloader.Helpers;

public static class WindowHelper
{
    // Импортируем функции из user32.dll
    [DllImport("user32.dll")]
    public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool SetForegroundWindow(IntPtr hWnd);

    [DllImport("user32.dll")]
    public static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll")]
    private static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

    // Возможные значения showCmd
    private const int SW_SHOWMINIMIZED = 2;
    private const int SW_SHOWMAXIMIZED = 3;
    private const int SW_SHOWNORMAL = 1;

    // Структуры для работы с расположением окна
    [StructLayout(LayoutKind.Sequential)]
    private struct WINDOWPLACEMENT
    {
        public int length;
        public int flags;
        public int showCmd;
        public POINT ptMinPosition;
        public POINT ptMaxPosition;
        public RECT rcNormalPosition;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct POINT
    {
        public int x;
        public int y;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct RECT
    {
        public int left;
        public int top;
        public int right;
        public int bottom;
    }

    /// <summary>
    /// Восстанавливает окно (если оно свернуто) и перемещает его на передний план.
    /// </summary>
    /// <param name="window">Объект окна приложения (WinUI 3 Window).</param>
    public static void BringWindowToFront(Window window)
    {
        // Получаем дескриптор окна
        IntPtr hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);

        // Восстанавливаем окно, если оно свернуто. Здесь 9 соответствует команде "restore" (SW_RESTORE).
        ShowWindow(hWnd, 9);

        // Перемещаем окно на передний план
        SetForegroundWindow(hWnd);
    }

    /// <summary>
    /// Проверяет, является ли окно активным (на переднем плане).
    /// </summary>
    /// <param name="hWnd">Дескриптор окна.</param>
    /// <returns>True, если окно активно; иначе false.</returns>
    public static bool IsWindowActive(IntPtr hWnd)
    {
        return hWnd == GetForegroundWindow();
    }

    /// <summary>
    /// Проверяет, свернуто ли окно.
    /// </summary>
    /// <param name="hWnd">Дескриптор окна.</param>
    /// <returns>True, если окно свернуто; иначе false.</returns>
    public static bool IsWindowMinimized(IntPtr hWnd)
    {
        WINDOWPLACEMENT placement = new WINDOWPLACEMENT();
        placement.length = Marshal.SizeOf(typeof(WINDOWPLACEMENT));
        if (GetWindowPlacement(hWnd, ref placement))
        {
            return placement.showCmd == SW_SHOWMINIMIZED;
        }
        return false;
    }
}