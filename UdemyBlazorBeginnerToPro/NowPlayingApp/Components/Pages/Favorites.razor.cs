using Microsoft.AspNetCore.Components;
using NowPlayingApp.Models;
using NowPlayingApp.Services;

namespace NowPlayingApp.Components.Pages;

/// <summary>
/// Page component that displays the user's favorite movies.
/// </summary>
public partial class Favorites : IDisposable
{
    private List<MovieResponse>? _favoriteMovies;
    private bool _isLoading;

    /// <summary>
    /// Gets or sets the service used to load and track favorite movies.
    /// </summary>
    [Inject]
    public IFavoritesService FavoritesService { get; set; } = null!;

    /// <summary>
    /// Gets or sets the logger used for favorites load errors.
    /// </summary>
    [Inject]
    public ILogger<Favorites> Logger { get; set; } = null!;

    internal List<MovieResponse>? FavoriteMovies => _favoriteMovies;

    internal bool IsLoading => _isLoading;

    /// <summary>
    /// Unsubscribes from favorites change notifications.
    /// </summary>
    public void Dispose()
    {
        FavoritesService.FavoritesChanged -= HandleFavoritesChanged;
    }

    internal async Task HandleFavoritesChangedForTestAsync() => await LoadFavoritesAsync();

    internal async Task LoadFavoritesForTestAsync() => await LoadFavoritesAsync();

    protected override async Task OnInitializedAsync()
    {
        FavoritesService.FavoritesChanged += HandleFavoritesChanged;
        await LoadFavoritesAsync();
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
            _favoriteMovies = await FavoritesService.GetFavoritesAsync();
        }
        catch (Exception exception)
        {
            Logger.LogError(
                exception,
                $"{nameof(FavoritesService.GetFavoritesAsync)} - an error occurred."
            );
        }
        finally
        {
            _isLoading = false;
        }
    }
}
