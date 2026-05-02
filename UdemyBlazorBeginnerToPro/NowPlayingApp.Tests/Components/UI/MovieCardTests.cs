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
        var options = Substitute.For<IOptions<TMDBClientSettings>>();
        options.Value.Returns(
            new TMDBClientSettings { TMDBImageBaseAddress = "https://image.tmdb.org/t/p/w500" }
        );
        var client = new TMDBClient(new HttpClient(), options);
        var sut = CreateMovieCardWithRemotePoster();

        Assert.True(sut.IsPosterLoading);
    }

    [Fact]
    public void IsPosterLoading_When_PosterLoadEventHandled_Returns_False()
    {
        var sut = CreateMovieCardWithRemotePoster();
        sut.ApplyOnParametersSetForTest();
        Assert.True(sut.IsPosterLoading);

        sut.HandlePosterLoad();

        Assert.False(sut.IsPosterLoading);
    }

    [Fact]
    public void IsPosterLoading_When_PosterErrorEventHandled_Returns_False()
    {
        var sut = CreateMovieCardWithRemotePoster();
        sut.ApplyOnParametersSetForTest();
        Assert.Equal(
            "https://image.tmdb.org/t/p/w500/9Z2uDYXqJrlmePznQQJhL6d92Rq.jpg",
            sut.PosterImageSrc
        );

        sut.HandlePosterError();

        Assert.False(sut.IsPosterLoading);
        Assert.Equal("/images/poster.png", sut.PosterImageSrc);
    }

    private static MovieCard CreateMovieCardWithRemotePoster()
    {
        var options = Substitute.For<IOptions<TMDBClientSettings>>();

        options.Value.Returns(
            new TMDBClientSettings { TMDBImageBaseAddress = "https://image.tmdb.org/t/p/w500" }
        );

        var client = new TMDBClient(new HttpClient(), options);
        var favoritesService = Substitute.For<FavoritesService>();

        return new MovieCard(favoritesService)
        {
            TMDBClient = client,
            Movie = new MovieResponse
            {
                Title = "Example Movie",
                PosterPath = "/9Z2uDYXqJrlmePznQQJhL6d92Rq.jpg",
            },
        };
    }
}
