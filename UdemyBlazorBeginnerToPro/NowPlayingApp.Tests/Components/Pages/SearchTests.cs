using System.Net;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NowPlayingApp.Components.Pages;
using NowPlayingApp.Services;
using NSubstitute;

namespace NowPlayingApp.Tests.Components.Pages;

public class SearchTests
{
    [Fact]
    public async Task OnParametersSetAsync_When_QueryMissing_DoesNotCallTMDB()
    {
        // Arrange
        var handler = new StubHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK));
        var sut = CreateSut(query: null, handler);

        // Act
        await sut.ApplyOnParametersSetAsyncForTest();

        // Assert
        Assert.Equal(0, handler.CallCount);
        Assert.Null(sut.SearchResults);
        Assert.False(sut.IsLoading);
    }

    [Fact]
    public async Task OnParametersSetAsync_When_QueryProvided_LoadsSearchResults()
    {
        // Arrange
        var handler = new StubHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(
                """
                {
                    "page": 1,
                    "results": [
                        { "id": 101, "title": "Interstellar", "release_date": "2014-11-07", "poster_path": "/x.jpg" }
                    ],
                    "total_pages": 1,
                    "total_results": 1
                }
                """,
                Encoding.UTF8,
                "application/json"
            ),
        });
        var sut = CreateSut("interstellar", handler);

        // Act
        await sut.ApplyOnParametersSetAsyncForTest();

        // Assert
        Assert.Equal(1, handler.CallCount);
        Assert.NotNull(sut.SearchResults);
        Assert.Single(sut.SearchResults);
        Assert.Equal(101, sut.SearchResults[0].Id);
        Assert.False(sut.IsLoading);
    }

    [Fact]
    public async Task OnParametersSetAsync_When_TMDBRequestFails_DoesNotThrow()
    {
        // Arrange
        var handler = new StubHttpMessageHandler(_ => new HttpResponseMessage(
            HttpStatusCode.InternalServerError
        ));
        var sut = CreateSut("dune", handler);

        // Act
        var action = async () => await sut.ApplyOnParametersSetAsyncForTest();

        // Assert
        await action();
        Assert.Null(sut.SearchResults);
        Assert.False(sut.IsLoading);
    }

    [Fact]
    public async Task OnParametersSetAsync_When_RequestCancelled_DoesNotThrow()
    {
        // Arrange
        var handler = new StubHttpMessageHandler(_ =>
            throw new OperationCanceledException("cancelled")
        );
        var sut = CreateSut("matrix", handler);

        // Act
        var action = async () => await sut.ApplyOnParametersSetAsyncForTest();

        // Assert
        await action();
        Assert.Null(sut.SearchResults);
        Assert.False(sut.IsLoading);
    }

    [Fact]
    public void Dispose_When_CalledMultipleTimes_DoesNotThrow()
    {
        // Arrange
        var handler = new StubHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK));
        var sut = CreateSut("avatar", handler);

        // Act & Assert
        sut.Dispose();
        sut.Dispose();
    }

    private static Search CreateSut(string? query, HttpMessageHandler handler)
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
        var logger = Substitute.For<ILogger<Search>>();

        return new Search
        {
            Query = query,
            TMDBClient = tmdbClient,
            Logger = logger,
        };
    }

    private sealed class StubHttpMessageHandler(
        Func<HttpRequestMessage, HttpResponseMessage> responseFactory
    ) : HttpMessageHandler
    {
        public int CallCount { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken
        )
        {
            CallCount++;
            return Task.FromResult(responseFactory(request));
        }
    }
}
