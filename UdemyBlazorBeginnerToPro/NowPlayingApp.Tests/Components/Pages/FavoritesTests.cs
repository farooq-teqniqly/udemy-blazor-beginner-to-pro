using Microsoft.Extensions.Logging;
using NowPlayingApp.Components.Pages;
using NowPlayingApp.Models;
using NowPlayingApp.Services;
using NSubstitute;

namespace NowPlayingApp.Tests.Components.Pages;

public class FavoritesTests
{
    [Fact]
    public async Task LoadFavoritesForTestAsync_When_Called_Updates_FavoriteMovies()
    {
        // Arrange
        var favoritesService = Substitute.For<IFavoritesService>();
        var logger = Substitute.For<ILogger<Favorites>>();
        var initialFavorites = new List<MovieResponse> { new() { Id = 10, Title = "Movie 10" } };
        favoritesService.GetFavoritesAsync().Returns(initialFavorites);
        var sut = new Favorites(favoritesService, logger);

        // Act
        await sut.LoadFavoritesForTestAsync();

        // Assert
        Assert.Single(sut.FavoriteMovies!);
        Assert.Equal(10, sut.FavoriteMovies![0].Id);
        Assert.False(sut.IsLoading);
    }

    [Fact]
    public async Task HandleFavoritesChangedForTestAsync_When_ListChanges_Refreshes_FavoriteMovies()
    {
        // Arrange
        var favoritesService = Substitute.For<IFavoritesService>();
        var logger = Substitute.For<ILogger<Favorites>>();
        var firstLoad = new List<MovieResponse> { new() { Id = 10, Title = "Movie 10" } };
        var refreshedLoad = new List<MovieResponse>();
        favoritesService.GetFavoritesAsync().Returns(firstLoad, refreshedLoad);
        var sut = new Favorites(favoritesService, logger);

        // Act
        await sut.LoadFavoritesForTestAsync();
        await sut.HandleFavoritesChangedForTestAsync();

        // Assert
        Assert.Empty(sut.FavoriteMovies!);
        Assert.False(sut.IsLoading);
    }
}
