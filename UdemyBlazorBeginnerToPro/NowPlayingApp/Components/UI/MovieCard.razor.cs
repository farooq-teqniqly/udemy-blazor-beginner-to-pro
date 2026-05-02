using Microsoft.AspNetCore.Components;
using NowPlayingApp.Models;
using NowPlayingApp.Services;

namespace NowPlayingApp.Components.UI;

public partial class MovieCard
{
    private readonly FavoritesService _favoritesService;

    [Inject]
    public TMDBClient TMDBClient { get; set; } = null!;

    [Parameter, EditorRequired]
    public MovieResponse Movie { get; set; } = null!;

    private bool _isPosterLoading = true;
    private string _posterSrc = string.Empty;

    internal bool IsPosterLoading => _isPosterLoading;

    internal string PosterImageSrc => _posterSrc;

    public MovieCard(FavoritesService favoritesService)
    {
        ArgumentNullException.ThrowIfNull(favoritesService);

        _favoritesService = favoritesService;
    }

    internal void ApplyOnParametersSetForTest() => OnParametersSet();

    protected override void OnParametersSet()
    {
        var newSrc = GetPosterUriString(Movie.PosterPath);
        if (!string.Equals(newSrc, _posterSrc, StringComparison.Ordinal))
        {
            _posterSrc = newSrc;
            _isPosterLoading = true;
        }
    }

    internal void HandlePosterLoad() => _isPosterLoading = false;

    internal void HandlePosterError()
    {
        var fallback = GetFallbackPosterPath();
        if (string.Equals(_posterSrc, fallback, StringComparison.Ordinal))
        {
            _isPosterLoading = false;
            return;
        }

        _posterSrc = fallback;
        _isPosterLoading = false;
    }

    private string GetFallbackPosterPath() => GetPosterUriString(string.Empty);

    private string GetFormattedDate(string rawDate) =>
        DateTime.TryParse(rawDate, out var parsedDate)
            ? parsedDate.ToString("MMMM dd, yyyy")
            : "Unknown";

    private string GetPosterUriString(string posterPath) =>
        TMDBClient.GetPosterUri(posterPath).ToString();

    private async Task HandleToggleFavorite()
    {
        await _favoritesService.AddFavoriteAsync(Movie);
    }
}
