using System;

namespace FluentDownloader.Services.Dependencies.Exceptions
{
    /// <summary>
    /// Represents an exception that is thrown when an unknown error occurs.
    /// </summary>
    public class UnknownErrorException : Exception
    {
        public UnknownErrorException()
            : base("An unknown error has occurred.") { }

        public UnknownErrorException(string message)
            : base(message) { }

        public UnknownErrorException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
