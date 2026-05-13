using System.Text.Json.Serialization;

namespace NowPlayingApp.Models
{
    /// <summary>
    /// Represents a language spoken in a movie, as returned by TMDB.
    /// </summary>
    public class SpokenLanguage
    {
        /// <summary>
        /// Gets or sets the English name of the language.
        /// </summary>
        [JsonPropertyName("english_name")]
        public string EnglishName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the ISO 639-1 language code.
        /// </summary>
        [JsonPropertyName("iso_639_1")]
        public string Iso6391 { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the native name of the language.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
    }
}
