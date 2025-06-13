using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentDownloader.Helpers
{
    public static class ExceptionHandler
    {
        public static void HandleGeneralException(Exception exception)
        {
            Debug.WriteLine($"{exception.GetType().FullName}: {exception.Message}\n\t{exception.StackTrace}");
        }
    }
}
