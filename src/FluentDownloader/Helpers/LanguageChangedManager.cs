using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentDownloader.Helpers
{
    public static class LanguageChangedManager
    {
        public static event EventHandler? LanguageChanged;

        public static void OnLanguageChanged()
        {
            LanguageChanged?.Invoke(null, EventArgs.Empty);
        }
    }

}
