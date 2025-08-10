using System;
using System.Collections.Generic;

namespace FluentDownloader.Models
{
    /// <summary>
    /// Represents metadata and format information for a video.
    /// </summary>
    public struct VideoData
    {
        private Dictionary<int, List<VideoFormatInfo>> _videoFormats;
        private string _thumbnailUri;
        private string _title;
        private string _id;
        private string[] _errors;

        public Dictionary<int, List<VideoFormatInfo>> VideoFormats => _videoFormats;
        public string ThumbnailUri => _thumbnailUri;
        public string Title => _title;
        public string ID => _id;
        public string[] Errors => _errors;
        public string Url { get; private set; }
        public bool IsPlaylist { get; set; }

        public VideoData(
            Dictionary<int, List<VideoFormatInfo>> videoFormats,
            string thumbnailUri,
            string url,
            string title,
            string id,
            string[] errors)
        {
            _videoFormats = videoFormats ?? new Dictionary<int, List<VideoFormatInfo>>();
            _thumbnailUri = thumbnailUri ?? string.Empty;
            _title = title ?? string.Empty;
            _id = id ?? string.Empty;
            _errors = errors ?? Array.Empty<string>();
            Url = url ?? string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VideoData"/> struct with errors only.
        /// </summary>
        /// <param name="errors">An array of errors related to the video.</param>
        public VideoData(string[] errors)
        {
            _videoFormats = new Dictionary<int, List<VideoFormatInfo>>();
            _thumbnailUri = string.Empty;
            Url = string.Empty;
            _title = string.Empty;
            _id = string.Empty;
            _errors = errors ?? Array.Empty<string>();
        }

        /// <summary>
        /// Resets the video data to an empty state.
        /// </summary>
        public void ResetToNull()
        {
            _videoFormats = new Dictionary<int, List<VideoFormatInfo>>();
            _thumbnailUri = string.Empty;
            _title = string.Empty;
            _id = string.Empty;
            Url = string.Empty;
            _errors = Array.Empty<string>();
        }
    }
}
