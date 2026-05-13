using NowPlayingApp.Components.Pages;

namespace NowPlayingApp.Tests.Components.Pages;

public class MovieDetailsFormatterTests
{
    [Theory]
    [InlineData("2025-01-15", "January 15, 2025")]
    [InlineData("2000-07-04", "July 04, 2000")]
    [InlineData("not-a-date", "")]
    [InlineData("", "")]
    public void FormatDateString_Returns_ExpectedString(string input, string expected)
    {
        // Act & Assert
        Assert.Equal(expected, MovieDetailsFormatter.FormatDateString(input));
    }

    [Theory]
    [InlineData(0, "0")]
    [InlineData(999, "999")]
    [InlineData(1000, "1,000")]
    [InlineData(1001, "1K")]
    [InlineData(1500, "2K")]
    [InlineData(2500, "2K")]
    [InlineData(10000, "10K")]
    public void FormatUserVotesString_Returns_ExpectedString(int voteCount, string expected)
    {
        // Act & Assert
        Assert.Equal(expected, MovieDetailsFormatter.FormatUserVotesString(voteCount));
    }

    [Theory]
    [InlineData(0, "")]
    [InlineData(1, "1 minute")]
    [InlineData(2, "2 minutes")]
    [InlineData(59, "59 minutes")]
    [InlineData(60, "1 hour ")]
    [InlineData(61, "1 hour 1 minute")]
    [InlineData(90, "1 hour 30 minutes")]
    [InlineData(120, "2 hours ")]
    [InlineData(121, "2 hours 1 minute")]
    public void GetRuntimeString_Returns_ExpectedString(int duration, string expected)
    {
        // Act & Assert
        Assert.Equal(expected, MovieDetailsFormatter.GetRuntimeString(duration));
    }

    [Theory]
    [InlineData(0.0f, "0%")]
    [InlineData(5.0f, "50%")]
    [InlineData(7.5f, "75%")]
    [InlineData(10.0f, "100%")]
    public void ToPercentString_Returns_ExpectedString(float voteAverage, string expected)
    {
        // Act & Assert
        Assert.Equal(expected, MovieDetailsFormatter.ToPercentString(voteAverage));
    }
}
