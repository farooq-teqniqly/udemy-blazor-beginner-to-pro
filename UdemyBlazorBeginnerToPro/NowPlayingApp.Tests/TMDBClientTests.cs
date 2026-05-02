using Microsoft.Extensions.Options;
using NowPlayingApp.Services;
using NSubstitute;

namespace NowPlayingApp.Tests;

public class TMDBClientTests
{
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
}
