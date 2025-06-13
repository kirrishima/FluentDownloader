using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using System;

namespace FluentDownloader
{
    public class EventAggregator
    {
        // --------- Theme update --------- 
        public event Action<ElementTheme>? ThemeChanged;

        public void ChangeTheme(ElementTheme theme) => ThemeChanged?.Invoke(theme);

        // --------- Backdrop update --------- 
        public event Action<object, SystemBackdrop>? SystemBackdropChanged;

        public void ChangeSystemBackdrop(object sender, SystemBackdrop backdrop) => SystemBackdropChanged?.Invoke(sender, backdrop);

        // --------- Backdrop update --------- 
        public event Action<Type>? PageChangeRequested;

        public void ChangePage(Type page) => PageChangeRequested?.Invoke(page);

        // --------- Backdrop update --------- 
        public event Action? PageGoBackRequested;

        public void PageGoBack() => PageGoBackRequested?.Invoke();

        // --------- Backdrop update --------- 
        public event Action? RecreateMainFrameRequested;

        public void RecreateMainFrame() => RecreateMainFrameRequested?.Invoke();
    }
}
