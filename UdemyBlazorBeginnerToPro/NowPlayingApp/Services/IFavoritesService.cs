using NowPlayingApp.Models;

namespace NowPlayingApp.Services
{
    /// <summary>
    /// Defines operations for managing favorite movies in client storage.
    /// </summary>
    public interface IFavoritesService
    {
        /// <summary>
        /// Occurs after the favorites collection is changed and persisted.
        /// </summary>
        event EventHandler? FavoritesChanged;

        /// <summary>
        /// Adds a movie to favorites when it is not already present.
        /// </summary>
        /// <param name="movie">The movie to add.</param>
        Task AddFavoriteAsync(MovieResponse movie);

        /// <summary>
        /// Retrieves all favorite movies.
        /// </summary>
        /// <returns>A list of favorite movies.</returns>
        Task<List<MovieResponse>> GetFavoritesAsync();

        /// <summary>
        /// Determines whether the specified movie ID is currently favorited.
        /// </summary>
        /// <param name="movieId">The movie identifier.</param>
        /// <returns><see langword="true"/> when the movie is a favorite; otherwise, <see langword="false"/>.</returns>
        Task<bool> IsFavorite(int movieId);

        /// <summary>
        /// Removes a movie from favorites.
        /// </summary>
        /// <param name="movie">The movie to remove.</param>
        Task RemoveFavoriteAsync(MovieResponse movie);

        /// <summary>
        /// Replaces the current favorites collection and persists it.
        /// </summary>
        /// <param name="movies">The movies to store as favorites.</param>
        Task SaveFavoritesAsync(List<MovieResponse> movies);
    }
}
