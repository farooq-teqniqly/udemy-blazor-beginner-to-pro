using NowPlayingApp.Models;

namespace NowPlayingApp.Services
{
    /// <summary>
    /// Manages favorite movies persisted in browser local storage.
    /// </summary>
    public class FavoritesService : IFavoritesService
    {
        private const string localStorageKey = "favorites";
        private readonly LocalStorageService _localStorageService;
        private readonly ILogger<FavoritesService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="FavoritesService"/> class.
        /// </summary>
        /// <param name="localStorageService">The local storage abstraction.</param>
        /// <param name="logger">The logger used for persistence failures.</param>
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

        /// <inheritdoc />
        public event EventHandler? FavoritesChanged;

        /// <inheritdoc />
        public async Task AddFavoriteAsync(MovieResponse movie)
        {
            ArgumentNullException.ThrowIfNull(movie);

            var current = await GetFavoritesAsync();

            if (current.All(m => m != movie))
            {
                current.Add(movie);
                await SaveFavoritesAsync(current);
            }
        }

        /// <inheritdoc />
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
                _logger.LogError(e, "Error removing favorite movie from local storage.");
            }

            return movies;
        }

        /// <inheritdoc />
        public async Task<bool> IsFavorite(int movieId)
        {
            var current = await GetFavoritesAsync();
            return current.Any(m => m.Id == movieId);
        }

        /// <inheritdoc />
        public async Task RemoveFavoriteAsync(MovieResponse movie)
        {
            ArgumentNullException.ThrowIfNull(movie);

            try
            {
                var current = await GetFavoritesAsync();
                var updatedFavorites = current.Where(m => m.Id != movie.Id).ToList();

                if (updatedFavorites.Count != current.Count)
                {
                    await SaveFavoritesAsync(updatedFavorites);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error saving favorite movies to local storage.");
            }
        }

        /// <inheritdoc />
        public async Task SaveFavoritesAsync(List<MovieResponse> movies)
        {
            ArgumentNullException.ThrowIfNull(movies);

            try
            {
                await _localStorageService.SetItemAsync(localStorageKey, movies);
                NotifyFavoritesChanged();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error saving favorite movies to local storage.");
            }
        }

        private void NotifyFavoritesChanged() => FavoritesChanged?.Invoke(this, EventArgs.Empty);
    }
}
