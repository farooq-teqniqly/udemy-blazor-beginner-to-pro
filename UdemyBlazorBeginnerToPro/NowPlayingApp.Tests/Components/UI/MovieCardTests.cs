using Microsoft.Extensions.Options;
using NowPlayingApp.Components.UI;
using NowPlayingApp.Models;
using NowPlayingApp.Services;
using NSubstitute;

#pragma warning disable BL0005

namespace NowPlayingApp.Tests.Components.UI;

public class MovieCardTests
{
    [Fact]
    public void IsPosterLoading_When_CardRendered_Returns_True()
    {
        // Arrange
        var sut = CreateMovieCardWithRemotePoster();

        // Act
        var isPosterLoading = sut.IsPosterLoading;

        // Assert
        Assert.True(isPosterLoading);
    }

    [Fact]
    public async Task IsPosterLoading_When_PosterLoadEventHandled_Returns_False()
    {
        // Arrange
        var sut = CreateMovieCardWithRemotePoster();
        await sut.ApplyOnParametersSetAsyncForTest();
        Assert.True(sut.IsPosterLoading);

        // Act
        sut.HandlePosterLoad();

        // Assert
        Assert.False(sut.IsPosterLoading);
    }

    [Fact]
    public async Task IsPosterLoading_When_PosterErrorEventHandled_Returns_False()
    {
        // Arrange
        var sut = CreateMovieCardWithRemotePoster();
        await sut.ApplyOnParametersSetAsyncForTest();
        Assert.Equal(
            "https://image.tmdb.org/t/p/w500/9Z2uDYXqJrlmePznQQJhL6d92Rq.jpg",
            sut.PosterImageSrc
        );

        // Act
        sut.HandlePosterError();

        // Assert
        Assert.False(sut.IsPosterLoading);
        Assert.Equal("/images/poster.png", sut.PosterImageSrc);
    }

    [Fact]
    public async Task IsFavorite_When_AddFavoriteHandled_Returns_True()
    {
        // Arrange
        var favoritesService = Substitute.For<IFavoritesService>();
        favoritesService.IsFavorite(42).Returns(false, true);
        var sut = CreateMovieCardWithRemotePoster(favoritesService, 42);
        await sut.ApplyOnParametersSetAsyncForTest();

        // Act
        await sut.HandleToggleFavoriteAsyncForTest();

        // Assert
        Assert.True(sut.IsFavorite);
        await favoritesService.Received(1).AddFavoriteAsync(Arg.Is<MovieResponse>(m => m.Id == 42));
    }

    [Fact]
    public async Task IsFavorite_When_RemoveFavoriteHandled_Returns_False()
    {
        // Arrange
        var favoritesService = Substitute.For<IFavoritesService>();
        favoritesService.IsFavorite(42).Returns(true, false);
        var sut = CreateMovieCardWithRemotePoster(favoritesService, 42);
        await sut.ApplyOnParametersSetAsyncForTest();

        // Act
        await sut.HandleToggleFavoriteAsyncForTest();

        // Assert
        Assert.False(sut.IsFavorite);
        await favoritesService
            .Received(1)
            .RemoveFavoriteAsync(Arg.Is<MovieResponse>(m => m.Id == 42));
    }

    private static MovieCard CreateMovieCardWithRemotePoster(
        IFavoritesService? favoritesService = null,
        int movieId = 0
    )
    {
        var options = Substitute.For<IOptions<TMDBClientSettings>>();

        options.Value.Returns(
            new TMDBClientSettings { TMDBImageBaseAddress = "https://image.tmdb.org/t/p/w500" }
        );

        var client = new TMDBClient(new HttpClient(), options);
        var resolvedFavoritesService = favoritesService ?? Substitute.For<IFavoritesService>();

        return new MovieCard
        {
            FavoritesService = resolvedFavoritesService,
            TMDBClient = client,
            Movie = new MovieResponse
            {
                Id = movieId,
                Title = "Example Movie",
                PosterPath = "/9Z2uDYXqJrlmePznQQJhL6d92Rq.jpg",
            },
        };
    }
}
