using NowPlayingApp.Components.UI;
using NowPlayingApp.Models;

#pragma warning disable BL0005

namespace NowPlayingApp.Tests.Components.UI;

public class MovieListTests
{
    [Fact]
    public void Movies_When_DefaultConstructed_Returns_Null()
    {
        // Arrange
        var sut = new MovieList();

        // Act
        var movies = sut.Movies;

        // Assert
        Assert.Null(movies);
    }

    [Fact]
    public void IsLoading_When_DefaultConstructed_Returns_False()
    {
        // Arrange
        var sut = new MovieList();

        // Act
        var isLoading = sut.IsLoading;

        // Assert
        Assert.False(isLoading);
    }

    [Fact]
    public void CategoryLabel_When_DefaultConstructed_Returns_EmptyString()
    {
        // Arrange
        var sut = new MovieList();

        // Act
        var categoryLabel = sut.CategoryLabel;

        // Assert
        Assert.Equal(string.Empty, categoryLabel);
    }

    [Fact]
    public void Movies_When_Set_Returns_AssignedInstance()
    {
        // Arrange
        var movies = new List<MovieResponse>();
        var sut = new MovieList { Movies = movies };

        // Act
        var assignedMovies = sut.Movies;

        // Assert
        Assert.Same(movies, assignedMovies);
    }

    [Fact]
    public void IsLoading_When_Set_Returns_AssignedValue()
    {
        // Arrange
        var sut = new MovieList { IsLoading = true };

        // Act
        var isLoading = sut.IsLoading;

        // Assert
        Assert.True(isLoading);
    }

    [Fact]
    public void CategoryLabel_When_Set_Returns_AssignedValue()
    {
        // Arrange
        var sut = new MovieList { CategoryLabel = "Popular" };

        // Act
        var categoryLabel = sut.CategoryLabel;

        // Assert
        Assert.Equal("Popular", categoryLabel);
    }
}
