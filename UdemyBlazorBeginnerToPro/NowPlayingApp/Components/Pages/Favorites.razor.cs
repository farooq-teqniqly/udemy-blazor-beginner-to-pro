using Microsoft.AspNetCore.Components;
using NowPlayingApp.Models;
using NowPlayingApp.Services;

namespace NowPlayingApp.Components.Pages;

/// <summary>
/// Page component that displays the user's favorite movies.
/// </summary>
public partial class Favorites : IDisposable
{
    private readonly IFavoritesService _favoritesService;
    private readonly ILogger<Favorites> _logger;

    private List<MovieResponse>? _favoriteMovies;
    private bool _isLoading;

    /// <summary>
    /// Initializes a new instance of the <see cref="Favorites"/> class.
    /// </summary>
    /// <param name="favoritesService">Service used to load and track favorite movies.</param>
    /// <param name="logger">Logger used for favorites load errors.</param>
    public Favorites(IFavoritesService favoritesService, ILogger<Favorites> logger)
    {
        ArgumentNullException.ThrowIfNull(favoritesService);
        ArgumentNullException.ThrowIfNull(logger);

        _favoritesService = favoritesService;
        _logger = logger;
    }

    internal List<MovieResponse>? FavoriteMovies => _favoriteMovies;

    internal bool IsLoading => _isLoading;

    internal async Task LoadFavoritesForTestAsync() => await LoadFavoritesAsync();

    internal async Task HandleFavoritesChangedForTestAsync() => await LoadFavoritesAsync();

    protected override async Task OnInitializedAsync()
    {
        _favoritesService.FavoritesChanged += HandleFavoritesChanged;
        await LoadFavoritesAsync();
    }

    /// <summary>
    /// Unsubscribes from favorites change notifications.
    /// </summary>
    public void Dispose()
    {
        _favoritesService.FavoritesChanged -= HandleFavoritesChanged;
    }

    private void HandleFavoritesChanged(object? sender, EventArgs e)
    {
        _ = InvokeAsync(async () =>
        {
            await LoadFavoritesAsync();
            StateHasChanged();
        });
    }

    private async Task LoadFavoritesAsync()
    {
        _isLoading = true;

        try
        {
            _favoriteMovies = await _favoritesService.GetFavoritesAsync();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"{nameof(_favoritesService.GetFavoritesAsync)} - an error occurred.");
        }
        finally
        {
            _isLoading = false;
        }
    }
}
