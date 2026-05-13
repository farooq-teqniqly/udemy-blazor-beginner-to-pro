using System.Text;

namespace NowPlayingApp.Components.Pages;

/// <summary>
/// Provides formatting methods for movie detail display values.
/// </summary>
internal static class MovieDetailsFormatter
{
    /// <summary>
    /// Parses an ISO date string and returns it formatted as "Month dd, yyyy".
    /// </summary>
    /// <param name="date">Date string to parse.</param>
    /// <returns>Formatted date string, or <see cref="string.Empty"/> if <paramref name="date"/> cannot be parsed.</returns>
    internal static string FormatDateString(string date)
    {
        var parsed = DateTime.TryParse(date, out var parsedDate);
        return parsed ? parsedDate.ToString("MMMM dd, yyyy") : string.Empty;
    }

    /// <summary>
    /// Formats a vote count as a human-readable string. Counts above 1,000 are rounded
    /// to the nearest thousand and expressed with a "K" suffix.
    /// </summary>
    /// <param name="voteCount">Number of user votes.</param>
    /// <returns>Formatted vote count string (e.g., "842", "2K").</returns>
    internal static string FormatUserVotesString(int voteCount)
    {
        if (voteCount <= 1000)
        {
            return voteCount.ToString("N0");
        }

        var roundedVoteCount = (int)Math.Round(voteCount / 1000.0) * 1000;
        return $"{roundedVoteCount / 1000}K";
    }

    /// <summary>
    /// Converts a runtime in minutes to a human-readable hours-and-minutes string.
    /// </summary>
    /// <param name="duration">Runtime in minutes.</param>
    /// <returns>Formatted runtime string (e.g., "2 hours 15 minutes", "45 minutes").</returns>
    internal static string GetRuntimeString(int duration)
    {
        var hours = duration / 60;
        var minutes = duration % 60;

        var sb = new StringBuilder();

        if (hours > 0)
        {
            sb = sb.Append(hours);
            sb = sb.Append(hours == 1 ? " hour " : " hours ");
        }

        if (minutes > 0)
        {
            sb.Append(minutes);
            sb = sb.Append(minutes == 1 ? " minute" : " minutes");
        }

        return sb.ToString();
    }

    /// <summary>
    /// Converts a TMDB vote average (0-10 scale) to a percentage string.
    /// </summary>
    /// <param name="voteAverage">Vote average on a 0-10 scale.</param>
    /// <returns>Percentage string rounded to the nearest whole number (e.g., "75%").</returns>
    internal static string ToPercentString(float voteAverage) => $"{(voteAverage * 10):F0}%";
}
