namespace FluentDownloader.Models
{
    /// <summary>
    /// Provides constant values related to the FFmpeg and yt-dlp dependencies, 
    /// including executable names, download links, and argument options.
    /// </summary>
    internal static class DependenciesConstants
    {
        /// <summary>
        /// The filename of the FFmpeg executable.
        /// </summary>
        public const string FfmpegExecutable = "ffmpeg.exe";

        /// <summary>
        /// The base folder where FFmpeg is stored.
        /// </summary>
        public const string FfmpegBaseFolder = "ffmpeg";

        /// <summary>
        /// The filename of the downloaded FFmpeg archive.
        /// </summary>
        public const string FfmpegDownloadFilename = "ffmpeg.zip";

        /// <summary>
        /// Command-line argument to retrieve the FFmpeg version.
        /// </summary>
        public const string FfmpegVersionArgs = "-version";

        /// <summary>
        /// Direct download link for the latest FFmpeg build.
        /// </summary>
        public const string FfmpegLink = "https://github.com/yt-dlp/FFmpeg-Builds/releases/latest/download/ffmpeg-master-latest-win64-gpl.zip";

        /// <summary>
        /// GitHub page containing FFmpeg releases and documentation.
        /// </summary>
        public const string FfmpegGitHubPage = "https://github.com/yt-dlp/FFmpeg-Builds";

        /// <summary>
        /// The filename of the yt-dlp executable.
        /// </summary>
        public const string YtDlpExecutable = "yt-dlp.exe";

        /// <summary>
        /// The base folder where yt-dlp is stored.
        /// </summary>
        public const string YtDlpBaseFolder = "ytdlp";

        /// <summary>
        /// The filename of the downloaded yt-dlp binary.
        /// </summary>
        public const string YtDlpDownloadFilename = "ytdlp.exe";

        /// <summary>
        /// Command-line argument to retrieve the yt-dlp version.
        /// </summary>
        public const string YtDlpVersionArgs = "--version";

        /// <summary>
        /// Direct download link for the latest yt-dlp binary.
        /// </summary>
        public const string YtDlpLink = "https://github.com/yt-dlp/yt-dlp/releases/latest/download/yt-dlp.exe";

        /// <summary>
        /// GitHub page containing yt-dlp releases and documentation.
        /// </summary>
        public const string YtDlpGitHubPage = "https://github.com/yt-dlp/yt-dlp";
    }
}
