namespace FluentDownloader.Controls
{
    /// <summary>
    /// Represents a language with its corresponding code and display name.
    /// </summary>
    public class Language
    {
        /// <summary>
        /// Gets or sets the language code (e.g., "en", "fr", "de").
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the display name of the language (e.g., "English", "French").
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Language"/> class with the specified code and display name.
        /// </summary>
        /// <param name="code">The language code.</param>
        /// <param name="displayName">The human-readable name of the language.</param>
        public Language(string code, string displayName)
        {
            Code = code;
            DisplayName = $"{code}, {displayName}";
        }
    }
}
