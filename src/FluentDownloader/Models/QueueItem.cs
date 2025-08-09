using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentDownloader.Models
{
    public class QueueItem
    {
        public string Title { get; set; } = null!;
        public string Size { get; set; } = null!;
        public string Status { get; set; } = null!;
        public string Resolution { get; set; } = null!;
    }
}
