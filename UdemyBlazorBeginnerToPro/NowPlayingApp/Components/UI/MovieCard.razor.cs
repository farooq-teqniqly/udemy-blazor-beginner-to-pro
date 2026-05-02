using Microsoft.AspNetCore.Components;
using NowPlayingApp.Models;
using NowPlayingApp.Services;

namespace NowPlayingApp.Components.UI;

public partial class MovieCard
{
    private readonly IFavoritesService _favoritesService;

    private bool _isPosterLoading = true;
    private string _posterSrc = string.Empty;

    public MovieCard(IFavoritesService favoritesService)
    {
        ArgumentNullException.ThrowIfNull(favoritesService);

        _favoritesService = favoritesService;
    }

    [Parameter, EditorRequired]
    public MovieResponse Movie { get; set; } = null!;

    [Inject]
    public TMDBClient TMDBClient { get; set; } = null!;
    internal bool IsPosterLoading => _isPosterLoading;

    internal string PosterImageSrc => _posterSrc;

    internal void ApplyOnParametersSetForTest() => OnParametersSet();

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

    internal void HandlePosterLoad() => _isPosterLoading = false;

    protected override void OnParametersSet()
    {
        var newSrc = GetPosterUriString(Movie.PosterPath);
        if (!string.Equals(newSrc, _posterSrc, StringComparison.Ordinal))
        {
            _posterSrc = newSrc;
            _isPosterLoading = true;
        }
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
