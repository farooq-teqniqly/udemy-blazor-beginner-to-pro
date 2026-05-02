using System.Text.Json.Serialization;

namespace NowPlayingApp.Models
{
    /// <summary>
    /// Represents a paged TMDB response containing a list of movies.
    /// </summary>
    public class MovieListResponse
    {
        /// <summary>
        /// Gets or sets the current page number.
        /// </summary>
        [JsonPropertyName("page")]
        public int Page { get; set; }

        /// <summary>
        /// Gets or sets the movies returned for the current page.
        /// </summary>
        [JsonPropertyName("results")]
        public List<MovieResponse> Results { get; set; } = [];

        /// <summary>
        /// Gets or sets the total number of available pages.
        /// </summary>
        [JsonPropertyName("total_pages")]
        public int TotalPages { get; set; }

        /// <summary>
        /// Gets or sets the total number of matching results.
        /// </summary>
        [JsonPropertyName("total_results")]
        public int TotalResults { get; set; }
    }
}
