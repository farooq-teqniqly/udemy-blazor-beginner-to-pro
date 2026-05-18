using System.Text.Json.Serialization;

namespace NowPlayingApp.Models;

/// <summary>
/// Represents the TMDB response for a movie's video collection.
/// </summary>
public class MovieVideosResponse
{
    /// <summary>
    /// Gets or sets the TMDB movie identifier.
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the list of video entries associated with the movie.
    /// </summary>
    [JsonPropertyName("results")]
    public MovieVideo[] Results { get; set; } = [];
}
