using System.Runtime.InteropServices;

namespace FluentDownloader.Utils;

public static class DisplayHelper
{
    [DllImport("user32.dll")]
    private static extern int GetSystemMetrics(int nIndex);

    private const int SM_CXSCREEN = 0; // Ширина экрана
    private const int SM_CYSCREEN = 1; // Высота экрана

    public static (int Width, int Height) GetScreenResolution()
    {
        int width = GetSystemMetrics(SM_CXSCREEN);
        int height = GetSystemMetrics(SM_CYSCREEN);
        return (width, height);
    }

    public static (int Width, int Height) DisplaySizes
    {
        get
        {
            return GetScreenResolution();
        }
    }
}
