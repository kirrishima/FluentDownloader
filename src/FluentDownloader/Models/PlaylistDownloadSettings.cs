using System.Collections.Generic;

namespace FluentDownloader.Models
{
    public struct PlaylistDownloadSettings
    {
        public long? StartVideoIndex { get; set; }
        public long? EndVideoIndex { get; set; }
        public IReadOnlyList<int>? PlaylistItems { get; set; }
    }
}
