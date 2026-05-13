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
        // Arrange
        var sut = CreateSut(42, SuccessHandler());

        // Act & Assert
        sut.Dispose();
        sut.Dispose();
    }

    [Fact]
    public void HandleBackdropError_Sets_IsBackdropLoading_False()
    {
        // Arrange
        var sut = CreateSut(42, SuccessHandler());

        // Act
        sut.HandleBackdropError();

        // Assert
        Assert.False(sut.IsBackdropLoading);
    }

    [Fact]
    public void HandleBackdropLoad_Sets_IsBackdropLoading_False()
    {
        // Arrange
        var sut = CreateSut(42, SuccessHandler());

        // Act
        sut.HandleBackdropLoad();

        // Assert
        Assert.False(sut.IsBackdropLoading);
    }

    [Fact]
    public void HandlePosterError_Sets_IsPosterLoading_False()
    {
        // Arrange
        var sut = CreateSut(42, SuccessHandler());

        // Act
        sut.HandlePosterError();

        // Assert
        Assert.False(sut.IsPosterLoading);
    }

    [Fact]
    public void HandlePosterLoad_Sets_IsPosterLoading_False()
    {
        // Arrange
        var sut = CreateSut(42, SuccessHandler());

        // Act
        sut.HandlePosterLoad();

        // Assert
        Assert.False(sut.IsPosterLoading);
    }

    [Fact]
    public async Task OnParametersSetAsync_When_HttpRequestFails_Sets_MovieDetailResponse_Null()
    {
        // Arrange
        var handler = new StubHttpMessageHandler(_ => new HttpResponseMessage(
            HttpStatusCode.InternalServerError
        ));
        var sut = CreateSut(42, handler);

        // Act
        await sut.ApplyOnParametersSetAsyncForTest();

        // Assert
        Assert.Null(sut.MovieDetailResponse);
        Assert.False(sut.IsPageLoading);
    }

    [Fact]
    public async Task OnParametersSetAsync_When_MovieLoaded_Sets_BackdropSrc()
    {
        // Arrange
        var sut = CreateSut(42, SuccessHandler());

        // Act
        await sut.ApplyOnParametersSetAsyncForTest();

        // Assert
        Assert.Equal("https://image.tmdb.org/t/p/w500/backdrop.jpg", sut.BackdropSrc);
    }

    [Fact]
    public async Task OnParametersSetAsync_When_MovieLoaded_Sets_IsBackdropLoading_True()
    {
        // Arrange
        var sut = CreateSut(42, SuccessHandler());

        // Act
        await sut.ApplyOnParametersSetAsyncForTest();

        // Assert
        Assert.True(sut.IsBackdropLoading);
    }

    [Fact]
    public async Task OnParametersSetAsync_When_MovieLoaded_Sets_IsPageLoading_False()
    {
        // Arrange
        var sut = CreateSut(42, SuccessHandler());

        // Act
        await sut.ApplyOnParametersSetAsyncForTest();

        // Assert
        Assert.False(sut.IsPageLoading);
    }

    [Fact]
    public async Task OnParametersSetAsync_When_MovieLoaded_Sets_IsPosterLoading_True()
    {
        // Arrange
        var sut = CreateSut(42, SuccessHandler());

        // Act
        await sut.ApplyOnParametersSetAsyncForTest();

        // Assert
        Assert.True(sut.IsPosterLoading);
    }

    [Fact]
    public async Task OnParametersSetAsync_When_MovieLoaded_Sets_MovieDetailResponse()
    {
        // Arrange
        var sut = CreateSut(42, SuccessHandler());

        // Act
        await sut.ApplyOnParametersSetAsyncForTest();

        // Assert
        Assert.NotNull(sut.MovieDetailResponse);
        Assert.Equal(42, sut.MovieDetailResponse.Id);
    }

    [Fact]
    public async Task OnParametersSetAsync_When_MovieLoaded_Sets_PosterSrc()
    {
        // Arrange
        var sut = CreateSut(42, SuccessHandler());

        // Act
        await sut.ApplyOnParametersSetAsyncForTest();

        // Assert
        Assert.Equal("https://image.tmdb.org/t/p/w500/poster.jpg", sut.PosterSrc);
    }

    [Fact]
    public async Task OnParametersSetAsync_When_RequestCancelled_DoesNotThrow()
    {
        // Arrange
        var handler = new StubHttpMessageHandler(_ =>
            throw new OperationCanceledException("cancelled")
        );
        var sut = CreateSut(42, handler);

        // Act
        await sut.ApplyOnParametersSetAsyncForTest();

        // Assert
        Assert.Null(sut.MovieDetailResponse);
        Assert.False(sut.IsPageLoading);
    }

    [Fact]
    public async Task OnParametersSetAsync_When_SameMovieLoadedTwice_DoesNotResetPosterLoading()
    {
        // Arrange
        var sut = CreateSut(42, SuccessHandler());
        await sut.ApplyOnParametersSetAsyncForTest();
        sut.HandlePosterLoad();
        Assert.False(sut.IsPosterLoading);

        // Act
        await sut.ApplyOnParametersSetAsyncForTest();

        // Assert
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
