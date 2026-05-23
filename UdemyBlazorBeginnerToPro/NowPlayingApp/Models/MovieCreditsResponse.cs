using System.Text.Json.Serialization;

namespace NowPlayingApp.Models;

/// <summary>
/// Represents the TMDB response for a movie's cast and crew credits.
/// </summary>
public class MovieCreditsResponse
{
    /// <summary>
    /// Gets or sets the cast members credited in the movie.
    /// </summary>
    [JsonPropertyName("cast")]
    public List<MovieCastMember> Cast { get; set; } = [];

    /// <summary>
    /// Gets or sets the crew members credited in the movie.
    /// </summary>
    [JsonPropertyName("crew")]
    public List<MovieCrewMember> Crew { get; set; } = [];

    /// <summary>
    /// Gets or sets the TMDB movie identifier.
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }
}
