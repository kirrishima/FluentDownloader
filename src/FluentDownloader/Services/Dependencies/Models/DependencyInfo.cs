namespace FluentDownloader.Services.Dependencies.Models
{
    /// <summary>
    /// Represents information about a dependency, including its installation status, version, and file path.
    /// </summary>
    public class DependencyInfo
    {
        /// <summary>
        /// Gets or sets the file path of the dependency.
        /// </summary>
        public string? Path { get; set; }

        /// <summary>
        /// Gets or sets the version of the dependency.
        /// </summary>
        public string? Version { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the dependency is installed.
        /// </summary>
        public bool IsInstalled { get; set; }

        public void ResetToNull()
        {
            Path = null;
            Version = null;
            IsInstalled = false;
        }
    }
}
