using System.Text.Json.Serialization;

namespace NowPlayingApp.Models
{
    /// <summary>
    /// Represents a TMDB genre.
    /// </summary>
    public class Genre
    {
        /// <summary>
        /// Gets or sets the TMDB genre identifier.
        /// </summary>
        [JsonPropertyName("id")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the display name of the genre.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
    }
}
