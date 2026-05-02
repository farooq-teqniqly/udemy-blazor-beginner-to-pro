using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using NowPlayingApp.Models;

namespace NowPlayingApp.Services
{
    /// <summary>
    /// Client for retrieving movie data and poster URLs from TMDB.
    /// </summary>
    public sealed class TMDBClient
    {
        /// <summary>
        /// Sort field value for popularity-based movie discovery.
        /// </summary>
        public static SortByField Popularity = new("popularity");

        /// <summary>
        /// Sort field value for primary release date-based movie discovery.
        /// </summary>
        public static SortByField PrimaryReleaseDate = new("primary_release_date");
        private const string DateFormat = "yyyy-MM-dd";
        private readonly HttpClient _http;
        private readonly TMDBClientSettings _settings;

        /// <summary>
        /// Represents a TMDB sort field identifier.
        /// </summary>
        /// <param name="FieldName">The TMDB API sort field name.</param>
        public record SortByField(string FieldName);

        /// <summary>
        /// Initializes a new instance of the <see cref="TMDBClient"/> class.
        /// </summary>
        /// <param name="http">HTTP client configured for TMDB calls.</param>
        /// <param name="settings">Application settings that provide TMDB endpoints and token.</param>
        public TMDBClient(HttpClient http, IOptions<TMDBClientSettings> settings)
        {
            ArgumentNullException.ThrowIfNull(http);
            ArgumentNullException.ThrowIfNull(settings);

            _http = http;
            _settings = settings.Value;
        }

        /// <summary>
        /// Retrieves movies released within the requested date window sorted by release date.
        /// </summary>
        /// <param name="inLastDays">Number of days back from today to include.</param>
        /// <param name="cancellationToken">Cancellation token for the request.</param>
        /// <returns>A list response containing now-playing movies.</returns>
        public async Task<MovieListResponse> GetNowPlayingMovies(
            int inLastDays = 14,
            CancellationToken cancellationToken = default
        )
        {
            return await GetMovies(PrimaryReleaseDate, inLastDays, cancellationToken);
        }

        /// <summary>
        /// Retrieves movies released within the requested date window sorted by popularity.
        /// </summary>
        /// <param name="inLastDays">Number of days back from today to include.</param>
        /// <param name="cancellationToken">Cancellation token for the request.</param>
        /// <returns>A list response containing popular movies.</returns>
        public async Task<MovieListResponse> GetPopularMovies(
            int inLastDays = 14,
            CancellationToken cancellationToken = default
        )
        {
            return await GetMovies(Popularity, inLastDays, cancellationToken);
        }

        /// <summary>
        /// Builds a poster URI for a TMDB poster path or returns the local fallback poster.
        /// </summary>
        /// <param name="posterPath">Poster path returned by TMDB.</param>
        /// <returns>An absolute TMDB image URI or a relative fallback URI.</returns>
        public Uri GetPosterUri(string posterPath)
        {
            ArgumentException.ThrowIfNullOrEmpty(_settings.TMDBImageBaseAddress);

            if (string.IsNullOrEmpty(posterPath))
            {
                return new Uri("/images/poster.png", UriKind.Relative);
            }

            // TMDB poster paths often start with '/'. new Uri(base, "/x.jpg") replaces the base path
            // with an absolute-on-host path; normalize so /t/p/w500 is preserved.
            var baseUri = new Uri(_settings.TMDBImageBaseAddress.TrimEnd('/') + "/");
            var relativePath = posterPath.TrimStart('/');
            return new Uri(baseUri, relativePath);
        }

        /// <summary>
        /// Searches for movies matching the specified query string using the TMDB API.
        /// </summary>
        /// <param name="query">The search query string for movie titles.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
        /// <returns>
        /// A <see cref="MovieListResponse"/> containing the list of movies that match the search query.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Thrown if <paramref name="query"/> is null or empty.
        /// </exception>
        /// <exception cref="HttpRequestException">
        /// Thrown if the HTTP request fails or the response cannot be deserialized.
        /// </exception>
        public async Task<MovieListResponse> SearchMovies(
            string query,
            CancellationToken cancellationToken = default
        )
        {
            ArgumentException.ThrowIfNullOrEmpty(query);

            var requestUri =
                $"search/movie?query={query}&include_adult=false&language=en-US&page=1";

            return await GetMoviesAsync(requestUri, cancellationToken);
        }

        private async Task<MovieListResponse> GetMovies(
            SortByField sortByField,
            int inLastDays = 14,
            CancellationToken cancellationToken = default
        )
        {
            ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(inLastDays, 0);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(inLastDays, 30);

            var now = DateTime.UtcNow;
            var releaseDateGte = now.AddDays(-inLastDays).ToString(DateFormat);
            var releaseDateLte = now.ToString(DateFormat);

            var requestUri =
                $"discover/movie?language=en-US&page=1&sort_by={sortByField.FieldName}.desc&primary_release_date.gte={releaseDateGte}&primary_release_date.lte={releaseDateLte}&with_original_language=en";

            return await GetMoviesAsync(requestUri, cancellationToken);
        }

        private async Task<MovieListResponse> GetMoviesAsync(
            string requestUri,
            CancellationToken cancellationToken = default
        )
        {
            var response = await _http.GetAsync(requestUri, cancellationToken);
            response.EnsureSuccessStatusCode();

            var movieListResponse = await response.Content.ReadFromJsonAsync<MovieListResponse>(
                cancellationToken: cancellationToken
            );

            if (movieListResponse is null)
            {
                throw new HttpRequestException(
                    $"Could not serialize the response into a {nameof(MovieListResponse)} instance.",
                    null,
                    HttpStatusCode.BadRequest
                );
            }
            return movieListResponse;
        }
    }
}
