using System.Text.Json.Serialization;

namespace NowPlayingApp.Models;

/// <summary>
/// Represents a single video entry (trailer, teaser, clip, etc.) returned by TMDB for a movie.
/// </summary>
public class MovieVideo
{
    /// <summary>
    /// Gets or sets the ISO 3166-1 country code for the video.
    /// </summary>
    [JsonPropertyName("iso_3166_1")]
    public string Iso31661 { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the ISO 639-1 language code for the video.
    /// </summary>
    [JsonPropertyName("iso_639_1")]
    public string Iso6391 { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the platform-specific video identifier used to construct a playback URL.
    /// </summary>
    [JsonPropertyName("key")]
    public string Key { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the TMDB movie identifier.
    /// </summary>
    [JsonPropertyName("id")]
    public string MovieId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the display name of the video.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the video is an official release.
    /// </summary>
    [JsonPropertyName("official")]
    public bool Official { get; set; }

    /// <summary>
    /// Gets or sets the UTC date and time when the video was published.
    /// </summary>
    [JsonPropertyName("published_at")]
    public DateTimeOffset PublishedAt { get; set; }

    /// <summary>
    /// Gets or sets the hosting platform for the video (e.g., YouTube).
    /// </summary>
    [JsonPropertyName("site")]
    public string Site { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the video resolution in pixels (e.g., 1080, 2160).
    /// </summary>
    [JsonPropertyName("size")]
    public int Size { get; set; }

    /// <summary>
    /// Gets or sets the video category (e.g., Trailer, Teaser, Clip, Featurette).
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;
}
