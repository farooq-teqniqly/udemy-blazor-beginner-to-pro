using System.Text.Json.Serialization;

namespace NowPlayingApp.Models
{
    /// <summary>
    /// Represents a production company associated with a movie, as returned by TMDB.
    /// </summary>
    public class ProductionCompany
    {
        /// <summary>
        /// Gets or sets the TMDB production company identifier.
        /// </summary>
        [JsonPropertyName("id")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the logo image path, or <see langword="null"/> if no logo is available.
        /// </summary>
        [JsonPropertyName("logo_path")]
        public string? LogoPath { get; set; }

        /// <summary>
        /// Gets or sets the name of the production company.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the ISO 3166-1 alpha-2 country code of the company's origin country.
        /// </summary>
        [JsonPropertyName("origin_country")]
        public string OriginCountry { get; set; } = string.Empty;
    }
}
