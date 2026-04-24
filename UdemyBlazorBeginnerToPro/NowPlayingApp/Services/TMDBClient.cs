using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using NowPlayingApp.Models;

namespace NowPlayingApp.Services
{
    public sealed class TMDBClient
    {
        private const string DateFormat = "yyyy-MM-dd";
        private readonly HttpClient _http;
        private readonly TMDBClientSettings _settings;

        public TMDBClient(HttpClient http, IOptions<TMDBClientSettings> settings)
        {
            ArgumentNullException.ThrowIfNull(http);
            ArgumentNullException.ThrowIfNull(settings);

            _http = http;
            _settings = settings.Value;
        }

        public async Task<MovieListResponse> GetNowPlayingMovies(
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
                $"discover/movie?language=en-US&page=1&sort_by=popularity.desc&primary_release_date.gte={releaseDateGte}&primary_release_date.lte={releaseDateLte}&with_original_language=en";

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
    }
}
