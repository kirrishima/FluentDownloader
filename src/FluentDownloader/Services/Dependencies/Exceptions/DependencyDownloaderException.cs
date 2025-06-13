using System;

namespace FluentDownloader.Services.Dependencies.Exceptions
{
    /// <summary>
    /// Represents an exception that occurs during the dependency downloading process.
    /// </summary>
    public class DependencyDownloaderException : Exception
    {
        public DependencyDownloaderException()
            : base("An unknown error has occurred.") { }

        public DependencyDownloaderException(string message)
            : base(message) { }

        public DependencyDownloaderException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
