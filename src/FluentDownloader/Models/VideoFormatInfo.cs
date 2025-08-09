namespace FluentDownloader.Models
{
    /// <summary>
    /// Represents information about a specific video format.
    /// </summary>
    public struct VideoFormatInfo
    {
        public string Resolution { get; }
        public string Extension { get; }
        public string FormatId { get; }
        public bool HasAudio { get; }
        public float? FPS { get; }
        public int? Height { get; }
        public long? FileSize { get; }

        public VideoFormatInfo(string resolution, string extension, float? fps, int? height, string formatId, bool hasAudio, long? size)
        {
            Resolution = resolution;
            Extension = extension;
            FPS = fps;
            Height = height;
            FormatId = formatId;
            HasAudio = hasAudio;
            FileSize = size;
        }

        /// <summary>
        /// Returns a string representation of the video format.
        /// </summary>
        /// <returns>A formatted string containing resolution and extension information.</returns>
        public override string ToString()
        {
            return $"{Resolution}p. [src ext={Extension}]";
        }
    }
}
