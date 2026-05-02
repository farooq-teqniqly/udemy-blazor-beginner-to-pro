using NowPlayingApp.Models;

namespace NowPlayingApp.Services
{
    public interface IFavoritesService
    {
        event EventHandler? FavoritesChanged;

        Task AddFavoriteAsync(MovieResponse movie);
        Task<List<MovieResponse>> GetFavoritesAsync();
        Task<bool> IsFavorite(int movieId);
        Task RemoveFavoriteAsync(MovieResponse movie);
        Task SaveFavoritesAsync(List<MovieResponse> movies);
    }
}
