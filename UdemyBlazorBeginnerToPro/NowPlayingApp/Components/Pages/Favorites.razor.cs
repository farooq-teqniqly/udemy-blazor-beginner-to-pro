using Microsoft.AspNetCore.Components;
using NowPlayingApp.Models;
using NowPlayingApp.Services;

namespace NowPlayingApp.Components.Pages;

public partial class Favorites : IDisposable
{
    private readonly IFavoritesService _favoritesService;
    private readonly ILogger<Favorites> _logger;

    private List<MovieResponse>? _favoriteMovies;
    private bool _isLoading;

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
