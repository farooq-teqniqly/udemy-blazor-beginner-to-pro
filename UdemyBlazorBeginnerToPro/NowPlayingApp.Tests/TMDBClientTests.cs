using System.Net;
using System.Text;
using Microsoft.Extensions.Options;
using NowPlayingApp.Services;
using NSubstitute;

namespace NowPlayingApp.Tests;

public class TMDBClientTests
{
    [Fact]
    public async Task SearchMovies_When_QueryProvided_CallsSearchEndpointAndReturnsResults()
    {
        // Arrange
        const string query = "batman";
        var handler = new StubHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(
                """
                {
                    "page": 1,
                    "results": [
                        { "id": 42, "title": "Batman Begins", "release_date": "2005-06-15", "poster_path": "/abc.jpg" }
                    ],
                    "total_pages": 1,
                    "total_results": 1
                }
                """,
                Encoding.UTF8,
                "application/json"
            ),
        });

        var sut = CreateSut(handler);

        // Act
        var result = await sut.SearchMovies(query);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Results);
        Assert.Equal(42, result.Results[0].Id);
        Assert.Equal(
            "/3/search/movie?query=batman&include_adult=false&language=en-US&page=1",
            handler.RequestedUri?.PathAndQuery
        );
    }

    [Fact]
    public async Task SearchMovies_When_ResponseBodyIsNull_ThrowsHttpRequestException()
    {
        // Arrange
        var handler = new StubHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("null", Encoding.UTF8, "application/json"),
        });
        var sut = CreateSut(handler);

        // Act
        var action = async () => await sut.SearchMovies("inception");

        // Assert
        await Assert.ThrowsAsync<HttpRequestException>(action);
    }

    [Fact]
    public async Task SearchMovies_When_QueryEmpty_ThrowsArgumentException()
    {
        // Arrange
        var sut = CreateSut(
            new StubHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK))
        );

        // Act
        var action = async () => await sut.SearchMovies(string.Empty);

        // Assert
        await Assert.ThrowsAsync<ArgumentException>(action);
    }

    [Fact]
    public void GetPosterUri_When_PosterPath_Not_Null_Returns_TMDB_Poster_File_Path()
    {
        // Arrange
        var options = Substitute.For<IOptions<TMDBClientSettings>>();

        options.Value.Returns(
            new TMDBClientSettings { TMDBImageBaseAddress = "https://image.tmdb.org/t/p/w500" }
        );

        var sut = new TMDBClient(new HttpClient(), options);
        var posterPath = "/9Z2uDYXqJrlmePznQQJhL6d92Rq.jpg";

        // Act
        var posterUri = sut.GetPosterUri(posterPath);

        // Assert
        Assert.Equal($"{options.Value.TMDBImageBaseAddress}{posterPath}", posterUri.ToString());
    }

    [Fact]
    public void GetPosterUri_When_PosterPath_Null_Returns_Local_Poster_File_Path()
    {
        // Arrange
        var options = Substitute.For<IOptions<TMDBClientSettings>>();

        options.Value.Returns(
            new TMDBClientSettings { TMDBImageBaseAddress = "https://image.tmdb.org/t/p/w500" }
        );

        var sut = new TMDBClient(new HttpClient(), options);

        // Act
        var posterUri = sut.GetPosterUri(null!);

        // Assert
        Assert.Equal("/images/poster.png", posterUri.ToString());
    }

    [Fact]
    public void GetPosterUri_When_TMDBImageBaseAddress_Null_Throws()
    {
        // Arrange
        var options = Substitute.For<IOptions<TMDBClientSettings>>();

        options.Value.Returns(new TMDBClientSettings { TMDBImageBaseAddress = null });

        var sut = new TMDBClient(new HttpClient(), options);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => sut.GetPosterUri("/poster.jpg"));
    }

    private static TMDBClient CreateSut(HttpMessageHandler handler)
    {
        var options = Substitute.For<IOptions<TMDBClientSettings>>();
        options.Value.Returns(
            new TMDBClientSettings { TMDBImageBaseAddress = "https://image.tmdb.org/t/p/w500" }
        );

        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://api.themoviedb.org/3/"),
        };

        return new TMDBClient(httpClient, options);
    }

    private sealed class StubHttpMessageHandler(
        Func<HttpRequestMessage, HttpResponseMessage> responseFactory
    ) : HttpMessageHandler
    {
        public Uri? RequestedUri { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken
        )
        {
            RequestedUri = request.RequestUri;
            return Task.FromResult(responseFactory(request));
        }
    }
}
