using NowPlayingApp.Models;

namespace NowPlayingApp.Services
{
    public interface IFavoritesService
    {
        Task AddFavoriteAsync(MovieResponse movie);
        Task<List<MovieResponse>> GetFavoritesAsync();
        Task SaveFavoritesAsync(List<MovieResponse> movies);
    }
}
