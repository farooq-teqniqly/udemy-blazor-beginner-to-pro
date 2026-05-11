using System.Text.Json.Serialization;

namespace NowPlayingApp.Models
{
    /// <summary>
    /// Represents a country where a movie was produced, as returned by TMDB.
    /// </summary>
    public class ProductionCountry
    {
        /// <summary>
        /// Gets or sets the ISO 3166-1 alpha-2 country code.
        /// </summary>
        [JsonPropertyName("iso_3166_1")]
        public string Iso31661 { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the full name of the country.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
    }
}
