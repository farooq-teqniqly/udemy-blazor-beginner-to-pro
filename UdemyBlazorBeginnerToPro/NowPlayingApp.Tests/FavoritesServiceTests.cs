using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using NowPlayingApp.Models;
using NowPlayingApp.Services;
using NSubstitute;

namespace NowPlayingApp.Tests;

public class FavoritesServiceTests
{
    [Fact]
    public async Task AddFavoriteAsync_When_MovieAdded_Raises_FavoritesChanged()
    {
        var sut = CreateSut(out var jsRuntime);
        var movie = new MovieResponse { Id = 1, Title = "Movie 1" };
        var eventRaisedCount = 0;
        sut.FavoritesChanged += (_, _) => eventRaisedCount++;

        await sut.AddFavoriteAsync(movie);

        Assert.Equal(1, eventRaisedCount);
        var stored = jsRuntime.GetStoredMovies("favorites");
        Assert.Single(stored);
        Assert.Equal(1, stored[0].Id);
    }

    [Fact]
    public async Task AddFavoriteAsync_When_MovieWithSameIdExists_DoesNotDuplicate()
    {
        var sut = CreateSut(out var jsRuntime);
        await sut.SaveFavoritesAsync([new MovieResponse { Id = 7, Title = "Saved once" }]);
        var replacementReference = new MovieResponse { Id = 7, Title = "Saved twice reference" };

        await sut.AddFavoriteAsync(replacementReference);

        var stored = jsRuntime.GetStoredMovies("favorites");
        Assert.Single(stored);
        Assert.Equal(7, stored[0].Id);
    }

    [Fact]
    public async Task RemoveFavoriteAsync_When_MovieRemoved_Raises_FavoritesChanged()
    {
        var sut = CreateSut(out _);
        await sut.SaveFavoritesAsync(
            [
                new MovieResponse { Id = 1, Title = "Movie 1" },
                new MovieResponse { Id = 2, Title = "Movie 2" },
            ]
        );

        var eventRaisedCount = 0;
        sut.FavoritesChanged += (_, _) => eventRaisedCount++;

        await sut.RemoveFavoriteAsync(new MovieResponse { Id = 2, Title = "Movie 2" });

        Assert.Equal(1, eventRaisedCount);
    }

    private static FavoritesService CreateSut(out FakeJsRuntime jsRuntime)
    {
        jsRuntime = new FakeJsRuntime();
        var localStorageService = new LocalStorageService(jsRuntime);
        var logger = Substitute.For<ILogger<FavoritesService>>();
        return new FavoritesService(localStorageService, logger);
    }

    private sealed class FakeJsRuntime : IJSRuntime
    {
        private readonly Dictionary<string, string> _storage = [];

        public ValueTask<TValue> InvokeAsync<TValue>(
            string identifier,
            CancellationToken cancellationToken,
            object?[]? args
        ) => InvokeAsync<TValue>(identifier, args);

        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args)
        {
            if (identifier == "localStorage.getItem")
            {
                var key = args?.FirstOrDefault()?.ToString() ?? string.Empty;
                _storage.TryGetValue(key, out var json);
                return new ValueTask<TValue>((TValue)(object?)json!);
            }

            if (identifier == "localStorage.setItem")
            {
                var key = args?[0]?.ToString() ?? string.Empty;
                var value = args?[1]?.ToString() ?? string.Empty;
                _storage[key] = value;
                return new ValueTask<TValue>(default(TValue)!);
            }

            throw new InvalidOperationException($"Unexpected JS call: {identifier}");
        }

        public List<MovieResponse> GetStoredMovies(string key)
        {
            if (!_storage.TryGetValue(key, out var json) || string.IsNullOrWhiteSpace(json))
            {
                return [];
            }

            return System.Text.Json.JsonSerializer.Deserialize<List<MovieResponse>>(json) ?? [];
        }
    }
}
