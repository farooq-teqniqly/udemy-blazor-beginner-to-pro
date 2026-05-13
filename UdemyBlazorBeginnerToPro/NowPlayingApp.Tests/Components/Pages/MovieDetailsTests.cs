using System.Net;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NowPlayingApp.Components.Pages;
using NowPlayingApp.Services;
using NSubstitute;

#pragma warning disable BL0005

namespace NowPlayingApp.Tests.Components.Pages;

public class MovieDetailsTests
{
    [Fact]
    public void Dispose_When_CalledMultipleTimes_DoesNotThrow()
    {
        var sut = CreateSut(42, SuccessHandler());

        sut.Dispose();
        sut.Dispose();
    }

    [Fact]
    public void HandleBackdropError_Sets_IsBackdropLoading_False()
    {
        var sut = CreateSut(42, SuccessHandler());

        sut.HandleBackdropError();

        Assert.False(sut.IsBackdropLoading);
    }

    [Fact]
    public void HandleBackdropLoad_Sets_IsBackdropLoading_False()
    {
        var sut = CreateSut(42, SuccessHandler());

        sut.HandleBackdropLoad();

        Assert.False(sut.IsBackdropLoading);
    }

    [Fact]
    public void HandlePosterError_Sets_IsPosterLoading_False()
    {
        var sut = CreateSut(42, SuccessHandler());

        sut.HandlePosterError();

        Assert.False(sut.IsPosterLoading);
    }

    [Fact]
    public void HandlePosterLoad_Sets_IsPosterLoading_False()
    {
        var sut = CreateSut(42, SuccessHandler());

        sut.HandlePosterLoad();

        Assert.False(sut.IsPosterLoading);
    }

    [Fact]
    public async Task OnParametersSetAsync_When_HttpRequestFails_Sets_MovieDetailResponse_Null()
    {
        var handler = new StubHttpMessageHandler(_ => new HttpResponseMessage(
            HttpStatusCode.InternalServerError
        ));
        var sut = CreateSut(42, handler);

        await sut.ApplyOnParametersSetAsyncForTest();

        Assert.Null(sut.MovieDetailResponse);
        Assert.False(sut.IsPageLoading);
    }

    [Fact]
    public async Task OnParametersSetAsync_When_MovieLoaded_Sets_BackdropSrc()
    {
        var sut = CreateSut(42, SuccessHandler());

        await sut.ApplyOnParametersSetAsyncForTest();

        Assert.Equal("https://image.tmdb.org/t/p/w500/backdrop.jpg", sut.BackdropSrc);
    }

    [Fact]
    public async Task OnParametersSetAsync_When_MovieLoaded_Sets_IsBackdropLoading_True()
    {
        var sut = CreateSut(42, SuccessHandler());

        await sut.ApplyOnParametersSetAsyncForTest();

        Assert.True(sut.IsBackdropLoading);
    }

    [Fact]
    public async Task OnParametersSetAsync_When_MovieLoaded_Sets_IsPageLoading_False()
    {
        var sut = CreateSut(42, SuccessHandler());

        await sut.ApplyOnParametersSetAsyncForTest();

        Assert.False(sut.IsPageLoading);
    }

    [Fact]
    public async Task OnParametersSetAsync_When_MovieLoaded_Sets_IsPosterLoading_True()
    {
        var sut = CreateSut(42, SuccessHandler());

        await sut.ApplyOnParametersSetAsyncForTest();

        Assert.True(sut.IsPosterLoading);
    }

    [Fact]
    public async Task OnParametersSetAsync_When_MovieLoaded_Sets_MovieDetailResponse()
    {
        var sut = CreateSut(42, SuccessHandler());

        await sut.ApplyOnParametersSetAsyncForTest();

        Assert.NotNull(sut.MovieDetailResponse);
        Assert.Equal(42, sut.MovieDetailResponse.Id);
    }

    [Fact]
    public async Task OnParametersSetAsync_When_MovieLoaded_Sets_PosterSrc()
    {
        var sut = CreateSut(42, SuccessHandler());

        await sut.ApplyOnParametersSetAsyncForTest();

        Assert.Equal("https://image.tmdb.org/t/p/w500/poster.jpg", sut.PosterSrc);
    }

    [Fact]
    public async Task OnParametersSetAsync_When_RequestCancelled_DoesNotThrow()
    {
        var handler = new StubHttpMessageHandler(_ =>
            throw new OperationCanceledException("cancelled")
        );
        var sut = CreateSut(42, handler);

        await sut.ApplyOnParametersSetAsyncForTest();

        Assert.Null(sut.MovieDetailResponse);
        Assert.False(sut.IsPageLoading);
    }

    [Fact]
    public async Task OnParametersSetAsync_When_SameMovieLoadedTwice_DoesNotResetPosterLoading()
    {
        var sut = CreateSut(42, SuccessHandler());
        await sut.ApplyOnParametersSetAsyncForTest();
        sut.HandlePosterLoad();
        Assert.False(sut.IsPosterLoading);

        await sut.ApplyOnParametersSetAsyncForTest();

        Assert.False(sut.IsPosterLoading);
    }

    private static MovieDetails CreateSut(int movieId, HttpMessageHandler handler)
    {
        var options = Substitute.For<IOptions<TMDBClientSettings>>();
        options.Value.Returns(
            new TMDBClientSettings { TMDBImageBaseAddress = "https://image.tmdb.org/t/p/w500" }
        );

        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://api.themoviedb.org/3/"),
        };

        var tmdbClient = new TMDBClient(httpClient, options);
        var logger = Substitute.For<ILogger<MovieDetails>>();

        return new MovieDetails
        {
            TMDBClient = tmdbClient,
            Logger = logger,
            MovieId = movieId,
        };
    }

    private static StubHttpMessageHandler SuccessHandler() =>
        new(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(
                """
                {
                  "id": 42,
                  "title": "Test Movie",
                  "tagline": "A tagline",
                  "overview": "An overview",
                  "release_date": "2024-01-01",
                  "runtime": 120,
                  "vote_average": 7.5,
                  "vote_count": 1000,
                  "poster_path": "/poster.jpg",
                  "backdrop_path": "/backdrop.jpg",
                  "genres": [],
                  "production_companies": [],
                  "production_countries": [],
                  "spoken_languages": [],
                  "origin_country": []
                }
                """,
                Encoding.UTF8,
                "application/json"
            ),
        });

    private sealed class StubHttpMessageHandler(
        Func<HttpRequestMessage, HttpResponseMessage> responseFactory
    ) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken
        )
        {
            return Task.FromResult(responseFactory(request));
        }
    }
}
