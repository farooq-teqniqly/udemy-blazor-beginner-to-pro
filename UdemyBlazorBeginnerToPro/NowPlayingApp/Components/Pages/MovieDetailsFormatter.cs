using System.Text;

namespace NowPlayingApp.Components.Pages;

internal static class MovieDetailsFormatter
{
    internal static string FormatDateString(string date)
    {
        var parsed = DateTime.TryParse(date, out var parsedDate);
        return parsed ? parsedDate.ToString("MMMM dd, yyyy") : string.Empty;
    }

    internal static string FormatUserVotesString(int voteCount)
    {
        if (voteCount <= 1000)
        {
            return voteCount.ToString("N0");
        }

        var roundedVoteCount = (int)Math.Round(voteCount / 1000.0) * 1000;
        return $"{roundedVoteCount / 1000}K";
    }

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

    internal static string ToPercentString(float voteAverage) => $"{(voteAverage * 10):F0}%";
}
