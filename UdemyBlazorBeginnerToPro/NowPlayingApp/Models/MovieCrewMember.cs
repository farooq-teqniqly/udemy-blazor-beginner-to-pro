using System.Text.Json.Serialization;

namespace NowPlayingApp.Models;

/// <summary>
/// Represents a single crew member returned by the TMDB movie credits endpoint.
/// </summary>
public class MovieCrewMember
{
    /// <summary>
    /// Gets or sets a value indicating whether the person is marked as adult content.
    /// </summary>
    [JsonPropertyName("adult")]
    public bool Adult { get; set; }

    /// <summary>
    /// Gets or sets the unique credit identifier assigned by TMDB.
    /// </summary>
    [JsonPropertyName("credit_id")]
    public string CreditId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the production department the person worked in.
    /// </summary>
    [JsonPropertyName("department")]
    public string Department { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the gender code for the person (0 = not set, 1 = female, 2 = male).
    /// </summary>
    [JsonPropertyName("gender")]
    public int Gender { get; set; }

    /// <summary>
    /// Gets or sets the TMDB person identifier.
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the specific job title held by the person.
    /// </summary>
    [JsonPropertyName("job")]
    public string Job { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the department the person is primarily known for.
    /// </summary>
    [JsonPropertyName("known_for_department")]
    public string KnownForDepartment { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the display name of the person.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the original name of the person.
    /// </summary>
    [JsonPropertyName("original_name")]
    public string OriginalName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the popularity score returned by TMDB.
    /// </summary>
    [JsonPropertyName("popularity")]
    public float Popularity { get; set; }

    /// <summary>
    /// Gets or sets the profile image path, or <see langword="null"/> when no image is available.
    /// </summary>
    [JsonPropertyName("profile_path")]
    public string? ProfilePath { get; set; }
}
