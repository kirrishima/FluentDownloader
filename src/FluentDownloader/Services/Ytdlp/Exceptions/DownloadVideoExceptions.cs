using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentDownloader.Services.Ytdlp.Exceptions
{
    public class DownloadVideoException : Exception
    {
        public DownloadVideoException() { }
        public DownloadVideoException(string message) : base(message) { }
        public DownloadVideoException(string message, Exception inner) : base(message, inner) { }
    }
}
