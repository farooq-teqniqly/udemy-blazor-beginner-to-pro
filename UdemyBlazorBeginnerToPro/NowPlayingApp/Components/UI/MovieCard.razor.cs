using Microsoft.AspNetCore.Components;
using NowPlayingApp.Models;
using NowPlayingApp.Services;

namespace NowPlayingApp.Components.UI;

/// <summary>
/// Renders a movie card with poster and favorite toggle interactions.
/// </summary>
public partial class MovieCard : IDisposable
{
    private readonly IFavoritesService _favoritesService;

    private bool _isPosterLoading = true;
    private string _posterSrc = string.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="MovieCard"/> class.
    /// </summary>
    /// <param name="favoritesService">Service used to query and update favorite state.</param>
    public MovieCard(IFavoritesService favoritesService)
    {
        ArgumentNullException.ThrowIfNull(favoritesService);

        _favoritesService = favoritesService;
    }

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

    internal void ApplyOnParametersSetForTest() => OnParametersSet();

    internal async Task ApplyOnParametersSetAsyncForTest() => await OnParametersSetAsync();

    internal async Task HandleToggleFavoriteAsyncForTest() => await HandleToggleFavoriteAsync();

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

    protected override void OnInitialized()
    {
        _favoritesService.FavoritesChanged += HandleFavoritesChanged;
    }

    protected override async Task OnParametersSetAsync()
    {
        var newSrc = GetPosterUriString(Movie.PosterPath);
        if (!string.Equals(newSrc, _posterSrc, StringComparison.Ordinal))
        {
            _posterSrc = newSrc;
            _isPosterLoading = true;
        }

        IsFavorite = await _favoritesService.IsFavorite(Movie.Id);
    }

    private string GetFallbackPosterPath() => GetPosterUriString(string.Empty);

    private string GetFormattedDate(string rawDate) =>
        DateTime.TryParse(rawDate, out var parsedDate)
            ? parsedDate.ToString("MMMM dd, yyyy")
            : "Unknown";

    private string GetPosterUriString(string posterPath) =>
        TMDBClient.GetPosterUri(posterPath).ToString();

    private async Task HandleToggleFavoriteAsync()
    {
        if (IsFavorite)
        {
            await _favoritesService.RemoveFavoriteAsync(Movie);
        }
        else
        {
            await _favoritesService.AddFavoriteAsync(Movie);
        }

        await UpdateFavoriteStateAsync();
    }

    private void HandleFavoritesChanged(object? sender, EventArgs e)
    {
        _ = InvokeAsync(async () =>
        {
            await UpdateFavoriteStateAsync();
            StateHasChanged();
        });
    }

    private async Task UpdateFavoriteStateAsync()
    {
        IsFavorite = await _favoritesService.IsFavorite(Movie.Id);
    }

    /// <summary>
    /// Unsubscribes from favorites events when the component is disposed.
    /// </summary>
    public void Dispose()
    {
        _favoritesService.FavoritesChanged -= HandleFavoritesChanged;
    }
}
