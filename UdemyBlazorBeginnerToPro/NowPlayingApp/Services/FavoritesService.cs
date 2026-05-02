using NowPlayingApp.Models;

namespace NowPlayingApp.Services
{
    public class FavoritesService : IFavoritesService
    {
        private const string localStorageKey = "favorites";
        private readonly LocalStorageService _localStorageService;
        private readonly ILogger<FavoritesService> _logger;

        public FavoritesService(
            LocalStorageService localStorageService,
            ILogger<FavoritesService> logger
        )
        {
            ArgumentNullException.ThrowIfNull(localStorageService);
            ArgumentNullException.ThrowIfNull(logger);

            _localStorageService = localStorageService;
            _logger = logger;
        }

        public async Task AddFavoriteAsync(MovieResponse movie)
        {
            ArgumentNullException.ThrowIfNull(movie);

            var current = await GetFavoritesAsync();

            if (current.All(m => m != movie))
            {
                current.Add(movie);
                await _localStorageService.SetItemAsync(localStorageKey, current);
            }
        }

        public async Task<List<MovieResponse>> GetFavoritesAsync()
        {
            List<MovieResponse> movies = [];

            try
            {
                movies =
                    await _localStorageService.GetItemAsync<List<MovieResponse>>(localStorageKey)
                    ?? [];
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error getting favorite movies from local storage.");
            }

            return movies;
        }

        public async Task SaveFavoritesAsync(List<MovieResponse> movies)
        {
            ArgumentNullException.ThrowIfNull(movies);

            try
            {
                await _localStorageService.SetItemAsync(localStorageKey, movies);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error saving favorite movies to local storage.");
            }
        }
    }
}
