using Microsoft.AspNetCore.Components;
using NowPlayingApp.Models;
using NowPlayingApp.Services;

namespace NowPlayingApp.Components.Pages;

/// <summary>
/// Page component that displays movies matching a search query.
/// </summary>
public partial class Search : IDisposable
{
    private CancellationTokenSource? _cancellationTokenSource;
    private bool _isLoading;
    private List<MovieResponse>? _searchResults;

    /// <summary>
    /// Gets or sets the logger used for search telemetry and errors.
    /// </summary>
    [Inject]
    public ILogger<Search> Logger { get; set; } = null!;

    /// <summary>
    /// Gets or sets the query string used to search for movies.
    /// </summary>
    [SupplyParameterFromQuery(Name = "q")]
    public string? Query { get; set; }

    /// <summary>
    /// Gets or sets the TMDB client used to search movies.
    /// </summary>
    [Inject]
    public TMDBClient TMDBClient { get; set; } = null!;
    internal bool IsLoading => _isLoading;
    internal List<MovieResponse>? SearchResults => _searchResults;

    /// <summary>
    /// Cancels and disposes any in-flight search request.
    /// </summary>
    public void Dispose()
    {
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource?.Dispose();
    }

    internal async Task ApplyOnParametersSetAsyncForTest() => await OnParametersSetAsync();

    protected override async Task OnParametersSetAsync()
    {
        if (string.IsNullOrEmpty(Query))
        {
            Logger.LogDebug(
                $"{nameof(Search.OnInitializedAsync)} - received null or empty query string."
            );
            return;
        }

        _cancellationTokenSource = new CancellationTokenSource();
        _isLoading = true;

        try
        {
            var response = await TMDBClient.SearchMovies(Query, _cancellationTokenSource.Token);
            _searchResults = response.Results;
        }
        catch (OperationCanceledException)
        {
            Logger.LogDebug($"{nameof(TMDBClient.SearchMovies)} request was cancelled.");
        }
        catch (HttpRequestException httpRequestException)
        {
            Logger.LogError(
                httpRequestException,
                $"{nameof(TMDBClient.SearchMovies)} - an error occurred."
            );
        }
        finally
        {
            _isLoading = false;
        }
    }
}
