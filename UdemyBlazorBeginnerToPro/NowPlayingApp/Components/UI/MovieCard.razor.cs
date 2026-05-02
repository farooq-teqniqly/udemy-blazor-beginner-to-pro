using Microsoft.AspNetCore.Components;
using NowPlayingApp.Models;
using NowPlayingApp.Services;

namespace NowPlayingApp.Components.UI;

/// <summary>
/// Renders a movie card with poster and favorite toggle interactions.
/// </summary>
public partial class MovieCard : IDisposable
{
    private bool _isPosterLoading = true;
    private string _posterSrc = string.Empty;

    /// <summary>
    /// Gets or sets the service used to query and update favorite state.
    /// </summary>
    [Inject]
    public IFavoritesService FavoritesService { get; set; } = null!;

    /// <summary>
    /// Gets or sets a value indicating whether the current movie is a favorite.
    /// </summary>
    public bool IsFavorite { get; set; }

    /// <summary>
    /// Gets or sets the movie displayed by the card.
    /// </summary>
    [Parameter, EditorRequired]
    public MovieResponse Movie { get; set; } = null!;

    /// <summary>
    /// Gets or sets the TMDB client used to resolve poster URLs.
    /// </summary>
    [Inject]
    public TMDBClient TMDBClient { get; set; } = null!;
    internal bool IsPosterLoading => _isPosterLoading;

    internal string PosterImageSrc => _posterSrc;

    /// <summary>
    /// Unsubscribes from favorites events when the component is disposed.
    /// </summary>
    public void Dispose()
    {
        FavoritesService.FavoritesChanged -= HandleFavoritesChanged;
    }

    internal async Task ApplyOnParametersSetAsyncForTest() => await OnParametersSetAsync();

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

    internal async Task HandleToggleFavoriteAsyncForTest() => await HandleToggleFavoriteAsync();

    protected override void OnInitialized()
    {
        FavoritesService.FavoritesChanged += HandleFavoritesChanged;
    }

    protected override async Task OnParametersSetAsync()
    {
        var newSrc = GetPosterUriString(Movie.PosterPath);
        if (!string.Equals(newSrc, _posterSrc, StringComparison.Ordinal))
        {
            _posterSrc = newSrc;
            _isPosterLoading = true;
        }

        IsFavorite = await FavoritesService.IsFavorite(Movie.Id);
    }

    private string GetFallbackPosterPath() => GetPosterUriString(string.Empty);

    private string GetFormattedDate(string rawDate) =>
        DateTime.TryParse(rawDate, out var parsedDate)
            ? parsedDate.ToString("MMMM dd, yyyy")
            : "Unknown";

    private string GetPosterUriString(string posterPath) =>
        TMDBClient.GetPosterUri(posterPath).ToString();

    private void HandleFavoritesChanged(object? sender, EventArgs e)
    {
        _ = InvokeAsync(async () =>
        {
            await UpdateFavoriteStateAsync();
            StateHasChanged();
        });
    }

    private async Task HandleToggleFavoriteAsync()
    {
        if (IsFavorite)
        {
            await FavoritesService.RemoveFavoriteAsync(Movie);
        }
        else
        {
            await FavoritesService.AddFavoriteAsync(Movie);
        }

        await UpdateFavoriteStateAsync();
    }

    private async Task UpdateFavoriteStateAsync()
    {
        IsFavorite = await FavoritesService.IsFavorite(Movie.Id);
    }
}
