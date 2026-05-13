using System.Text.Json;
using NowPlayingApp.Models;

namespace NowPlayingApp.Tests.Models;

public class MovieDetailResponseTests
{
    private static readonly string Json = """
        {
          "adult": false,
          "backdrop_path": "/qjv21Iu0nHQ0crJpy6KKiW8LX1Y.jpg",
          "belongs_to_collection": {
            "id": 931431,
            "name": "Mortal Kombat (Reboot) Collection",
            "poster_path": "/fzMBv3Bu8sWdsCQJbtF0XhY4Q2X.jpg",
            "backdrop_path": "/mI8xgtdaKDwnfaZ6CahOaYKdfSU.jpg"
          },
          "budget": 80000000,
          "genres": [
            { "id": 28, "name": "Action" },
            { "id": 14, "name": "Fantasy" },
            { "id": 12, "name": "Adventure" }
          ],
          "homepage": "https://www.mortalkombatmovie.com",
          "id": 931285,
          "imdb_id": "tt17490712",
          "origin_country": [ "US" ],
          "original_language": "en",
          "original_title": "Mortal Kombat II",
          "overview": "The fan favorite champions.",
          "popularity": 224.6327,
          "poster_path": "/lIsMeDbwntNXSUVHmWMMRXEZOVc.jpg",
          "production_companies": [
            { "id": 12, "logo_path": "/2ycs64eqV5rqKYHyQK0GVoKGvfX.png", "name": "New Line Cinema", "origin_country": "US" },
            { "id": 252367, "logo_path": null, "name": "Fireside Films", "origin_country": "US" }
          ],
          "production_countries": [
            { "iso_3166_1": "US", "name": "United States of America" }
          ],
          "release_date": "2026-05-06",
          "revenue": 61500000,
          "runtime": 116,
          "spoken_languages": [
            { "english_name": "English", "iso_639_1": "en", "name": "English" }
          ],
          "status": "Released",
          "tagline": "Finish the fight.",
          "title": "Mortal Kombat II",
          "video": false,
          "vote_average": 7.201,
          "vote_count": 112
        }
        """;

    [Fact]
    public void Deserialize_When_BelongsToCollectionIsNull_MapsToNull()
    {
        // Arrange
        const string json = """{ "id": 1, "title": "Test", "belongs_to_collection": null }""";

        // Act
        var sut = JsonSerializer.Deserialize<MovieDetailResponse>(json);

        // Assert
        Assert.NotNull(sut);
        Assert.Null(sut.BelongsToCollection);
    }

    [Fact]
    public void Deserialize_When_FullJson_MapsBelongsToCollection()
    {
        // Act
        var sut = JsonSerializer.Deserialize<MovieDetailResponse>(Json);

        // Assert
        Assert.NotNull(sut?.BelongsToCollection);
        Assert.Equal(931431, sut.BelongsToCollection.Id);
        Assert.Equal("Mortal Kombat (Reboot) Collection", sut.BelongsToCollection.Name);
        Assert.Equal("/fzMBv3Bu8sWdsCQJbtF0XhY4Q2X.jpg", sut.BelongsToCollection.PosterPath);
        Assert.Equal("/mI8xgtdaKDwnfaZ6CahOaYKdfSU.jpg", sut.BelongsToCollection.BackdropPath);
    }

    [Fact]
    public void Deserialize_When_FullJson_MapsGenres()
    {
        // Act
        var sut = JsonSerializer.Deserialize<MovieDetailResponse>(Json);

        // Assert
        Assert.NotNull(sut);
        Assert.Equal(3, sut.Genres.Count);
        Assert.Equal(28, sut.Genres[0].Id);
        Assert.Equal("Action", sut.Genres[0].Name);
        Assert.Equal(14, sut.Genres[1].Id);
        Assert.Equal("Fantasy", sut.Genres[1].Name);
        Assert.Equal(12, sut.Genres[2].Id);
        Assert.Equal("Adventure", sut.Genres[2].Name);
    }

    [Fact]
    public void Deserialize_When_FullJson_MapsProductionCompanies()
    {
        // Act
        var sut = JsonSerializer.Deserialize<MovieDetailResponse>(Json);

        // Assert
        Assert.NotNull(sut);
        Assert.Equal(2, sut.ProductionCompanies.Count);
        Assert.Equal(12, sut.ProductionCompanies[0].Id);
        Assert.Equal("/2ycs64eqV5rqKYHyQK0GVoKGvfX.png", sut.ProductionCompanies[0].LogoPath);
        Assert.Equal("New Line Cinema", sut.ProductionCompanies[0].Name);
        Assert.Equal("US", sut.ProductionCompanies[0].OriginCountry);
    }

    [Fact]
    public void Deserialize_When_FullJson_MapsProductionCountries()
    {
        // Act
        var sut = JsonSerializer.Deserialize<MovieDetailResponse>(Json);

        // Assert
        Assert.NotNull(sut);
        Assert.Single(sut.ProductionCountries);
        Assert.Equal("US", sut.ProductionCountries[0].Iso31661);
        Assert.Equal("United States of America", sut.ProductionCountries[0].Name);
    }

    [Fact]
    public void Deserialize_When_FullJson_MapsSpokenLanguages()
    {
        // Act
        var sut = JsonSerializer.Deserialize<MovieDetailResponse>(Json);

        // Assert
        Assert.NotNull(sut);
        Assert.Single(sut.SpokenLanguages);
        Assert.Equal("English", sut.SpokenLanguages[0].EnglishName);
        Assert.Equal("en", sut.SpokenLanguages[0].Iso6391);
        Assert.Equal("English", sut.SpokenLanguages[0].Name);
    }

    [Fact]
    public void Deserialize_When_FullJson_MapsTopLevelProperties()
    {
        // Act
        var sut = JsonSerializer.Deserialize<MovieDetailResponse>(Json);

        // Assert
        Assert.NotNull(sut);
        Assert.False(sut.Adult);
        Assert.Equal("/qjv21Iu0nHQ0crJpy6KKiW8LX1Y.jpg", sut.BackdropPath);
        Assert.Equal(80000000L, sut.Budget);
        Assert.Equal("https://www.mortalkombatmovie.com", sut.Homepage);
        Assert.Equal(931285, sut.Id);
        Assert.Equal("tt17490712", sut.ImdbId);
        Assert.Equal(["US"], sut.OriginCountry);
        Assert.Equal("en", sut.OriginalLanguage);
        Assert.Equal("Mortal Kombat II", sut.OriginalTitle);
        Assert.Equal("The fan favorite champions.", sut.Overview);
        Assert.Equal(224.6327f, sut.Popularity, precision: 2);
        Assert.Equal("/lIsMeDbwntNXSUVHmWMMRXEZOVc.jpg", sut.PosterPath);
        Assert.Equal("2026-05-06", sut.ReleaseDate);
        Assert.Equal(61500000L, sut.Revenue);
        Assert.Equal(116, sut.Runtime);
        Assert.Equal("Released", sut.Status);
        Assert.Equal("Finish the fight.", sut.Tagline);
        Assert.Equal("Mortal Kombat II", sut.Title);
        Assert.False(sut.Video);
        Assert.Equal(7.201f, sut.VoteAverage, precision: 2);
        Assert.Equal(112, sut.VoteCount);
    }

    [Fact]
    public void Deserialize_When_ProductionCompanyLogoPathIsNull_MapsToNull()
    {
        // Act
        var sut = JsonSerializer.Deserialize<MovieDetailResponse>(Json);

        // Assert
        Assert.NotNull(sut);
        var company = sut.ProductionCompanies.Single(c => c.Id == 252367);
        Assert.Null(company.LogoPath);
        Assert.Equal("Fireside Films", company.Name);
    }
}
