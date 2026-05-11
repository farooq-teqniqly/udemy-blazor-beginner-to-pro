using System.Text.Json.Serialization;

namespace NowPlayingApp.Models
{
    /// <summary>
    /// Represents the franchise or series a movie belongs to, as returned by TMDB.
    /// </summary>
    public class MovieFranchise
    {
        /// <summary>
        /// Gets or sets the backdrop image path.
        /// </summary>
        [JsonPropertyName("backdrop_path")]
        public string BackdropPath { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the TMDB franchise identifier.
        /// </summary>
        [JsonPropertyName("id")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the franchise.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the poster image path.
        /// </summary>
        [JsonPropertyName("poster_path")]
        public string PosterPath { get; set; } = string.Empty;
    }
}
