using NowPlayingApp.Components.UI;
using NowPlayingApp.Models;

#pragma warning disable BL0005

namespace NowPlayingApp.Tests.Components.UI;

public class MovieListTests
{
    [Fact]
    public void Movies_When_DefaultConstructed_Returns_Null()
    {
        var sut = new MovieList();

        Assert.Null(sut.Movies);
    }

    [Fact]
    public void IsLoading_When_DefaultConstructed_Returns_False()
    {
        var sut = new MovieList();

        Assert.False(sut.IsLoading);
    }

    [Fact]
    public void CategoryLabel_When_DefaultConstructed_Returns_EmptyString()
    {
        var sut = new MovieList();

        Assert.Equal(string.Empty, sut.CategoryLabel);
    }

    [Fact]
    public void Movies_When_Set_Returns_AssignedInstance()
    {
        var movies = new MovieListResponse
        {
            Page = 1,
            TotalPages = 1,
            TotalResults = 0,
            Results = [],
        };
        var sut = new MovieList { Movies = movies };

        Assert.Same(movies, sut.Movies);
    }

    [Fact]
    public void IsLoading_When_Set_Returns_AssignedValue()
    {
        var sut = new MovieList { IsLoading = true };

        Assert.True(sut.IsLoading);
    }

    [Fact]
    public void CategoryLabel_When_Set_Returns_AssignedValue()
    {
        var sut = new MovieList { CategoryLabel = "Popular" };

        Assert.Equal("Popular", sut.CategoryLabel);
    }
}
