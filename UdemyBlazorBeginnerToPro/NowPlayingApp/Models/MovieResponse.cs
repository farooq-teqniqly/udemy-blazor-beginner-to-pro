using System.Text.Json.Serialization;

namespace NowPlayingApp.Models;

/// <summary>
/// Represents a movie item returned by TMDB.
/// </summary>
public class MovieResponse
{
    /// <summary>
    /// Gets or sets a value indicating whether the movie is marked as adult content.
    /// </summary>
    [JsonPropertyName("adult")]
    public bool Adult { get; set; }

    /// <summary>
    /// Gets or sets the backdrop image path.
    /// </summary>
    [JsonPropertyName("backdrop_path")]
    public string BackdropPath { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the TMDB genre identifiers associated with the movie.
    /// </summary>
    [JsonPropertyName("genre_ids")]
    public int[] GenreIds { get; set; } = [];

    /// <summary>
    /// Gets or sets the TMDB movie identifier.
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the original language code for the movie.
    /// </summary>
    [JsonPropertyName("original_language")]
    public string OriginalLanguage { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the original title of the movie.
    /// </summary>
    [JsonPropertyName("original_title")]
    public string OriginalTitle { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the movie overview text.
    /// </summary>
    [JsonPropertyName("overview")]
    public string Overview { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the popularity score returned by TMDB.
    /// </summary>
    [JsonPropertyName("popularity")]
    public float Popularity { get; set; }

    /// <summary>
    /// Gets or sets the poster image path.
    /// </summary>
    [JsonPropertyName("poster_path")]
    public string PosterPath { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the release date string returned by TMDB.
    /// </summary>
    [JsonPropertyName("release_date")]
    public string ReleaseDate { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the display title of the movie.
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the item is a video.
    /// </summary>
    [JsonPropertyName("video")]
    public bool Video { get; set; }

    /// <summary>
    /// Gets or sets the average vote score.
    /// </summary>
    [JsonPropertyName("vote_average")]
    public float VoteAverage { get; set; }

    /// <summary>
    /// Gets or sets the number of votes received.
    /// </summary>
    [JsonPropertyName("vote_count")]
    public int VoteCount { get; set; }

    /// <summary>
    /// Determines whether two movie instances are not equal.
    /// </summary>
    /// <param name="left">The left movie operand.</param>
    /// <param name="right">The right movie operand.</param>
    /// <returns><see langword="true"/> when the instances are not equal; otherwise, <see langword="false"/>.</returns>
    public static bool operator !=(MovieResponse? left, MovieResponse? right)
    {
        return !(left == right);
    }

    /// <summary>
    /// Determines whether two movie instances are equal by ID.
    /// </summary>
    /// <param name="left">The left movie operand.</param>
    /// <param name="right">The right movie operand.</param>
    /// <returns><see langword="true"/> when the instances are equal; otherwise, <see langword="false"/>.</returns>
    public static bool operator ==(MovieResponse? left, MovieResponse? right)
    {
        if (ReferenceEquals(left, right))
        {
            return true;
        }

        if (left is null || right is null)
        {
            return false;
        }

        return left.Equals(right);
    }

    /// <summary>
    /// Determines whether this instance equals another object by comparing movie IDs.
    /// </summary>
    /// <param name="obj">The object to compare with this instance.</param>
    /// <returns><see langword="true"/> if the object is a movie with the same ID; otherwise, <see langword="false"/>.</returns>
    public override bool Equals(object? obj)
    {
        if (obj is MovieResponse other)
        {
            return Id == other.Id;
        }
        return false;
    }

    /// <summary>
    /// Returns a hash code for this movie based on its ID.
    /// </summary>
    /// <returns>A hash code for the current movie.</returns>
    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}
