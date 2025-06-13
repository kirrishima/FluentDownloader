using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentDownloader.Helpers
{
    public static class TeachingTipHelper
    {
        public static void Show(TeachingTip tip, TimeSpan duration, DispatcherQueue dispatcherQueue)
        {
            if (tip == null || dispatcherQueue == null) return;

            var timer = dispatcherQueue.CreateTimer();
            timer.Interval = duration;
            timer.Tick += (s, e) =>
            {
                tip.IsOpen = false;
                timer.Stop();
            };
            tip.IsOpen = true;
            timer.Start();
        }
    }
}
