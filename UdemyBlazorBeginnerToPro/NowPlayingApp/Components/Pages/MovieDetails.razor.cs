using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using NowPlayingApp.Models;
using NowPlayingApp.Services;

namespace NowPlayingApp.Components.Pages;

public partial class MovieDetails : IDisposable
{
    private string _backdropSrc = string.Empty;
    private CancellationTokenSource? _cancellationTokenSource;
    private List<MovieCastMember> _cast = [];
    private bool _isBackdropLoading = true;
    private bool _isPageLoading;
    private bool _isPosterLoading = true;
    private MovieDetailResponse? _movieDetailResponse;
    private string _posterSrc = string.Empty;
    private MovieVideo? _trailer;

    [Inject]
    public IJSRuntime JsRuntime { get; set; } = null!;

    [Inject]
    public ILogger<MovieDetails> Logger { get; set; } = null!;

    [Parameter]
    public int MovieId { get; set; }

    [Inject]
    public TMDBClient TMDBClient { get; set; } = null!;
    internal string BackdropSrc => _backdropSrc;
    internal bool IsBackdropLoading => _isBackdropLoading;
    internal bool IsPageLoading => _isPageLoading;
    internal bool IsPosterLoading => _isPosterLoading;
    internal MovieDetailResponse? MovieDetailResponse => _movieDetailResponse;
    internal string PosterSrc => _posterSrc;

    public void Dispose()
    {
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource?.Dispose();
    }

    internal async Task ApplyOnParametersSetAsyncForTest() => await OnParametersSetAsync();

    internal void HandleBackdropError() => _isBackdropLoading = false;

    internal void HandleBackdropLoad() => _isBackdropLoading = false;

    internal void HandlePosterError() => _isPosterLoading = false;

    internal void HandlePosterLoad() => _isPosterLoading = false;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await PlayTrailerAsync();
    }

    protected override async Task OnParametersSetAsync()
    {
        _cancellationTokenSource = new CancellationTokenSource();
        var token = _cancellationTokenSource.Token;
        _isPageLoading = true;

        var trailerTask = TMDBClient.GetTrailer(MovieId, token);
        var creditsTask = TMDBClient.GetCredits(MovieId, token);

        _movieDetailResponse = await ExecuteApiCallAsync(
            nameof(TMDBClient.GetMovieDetail),
            TMDBClient.GetMovieDetail(MovieId, token)
        );

        if (_movieDetailResponse is not null)
            UpdateImageSources();

        _isPageLoading = false;

        _trailer = await ExecuteApiCallAsync(nameof(TMDBClient.GetTrailer), trailerTask);

        var credits = await ExecuteApiCallAsync(nameof(TMDBClient.GetCredits), creditsTask);

        if (credits is not null)
        {
            _cast = [.. credits.Cast.OrderBy(c => c.Order)];

            Logger.LogDebug(
                "{Operation} - retrieved {CastMemberCount} cast members.",
                nameof(TMDBClient.GetCredits),
                _cast.Count
            );

            foreach (var castMember in _cast)
                castMember.ProfilePath = GetProfileUriString(castMember.ProfilePath);
        }
    }

    private async Task<T?> ExecuteApiCallAsync<T>(string operationName, Task<T> apiTask)
        where T : class?
    {
        try
        {
            return await apiTask;
        }
        catch (OperationCanceledException)
        {
            Logger.LogDebug("{Operation} request was cancelled.", operationName);
            return null;
        }
        catch (HttpRequestException ex)
        {
            Logger.LogError(ex, "{Operation} - an error occurred.", operationName);
            return null;
        }
    }

    private string GetBackdropUriString(string backdropPath) =>
        TMDBClient.GetBackdropUri(backdropPath).ToString();

    private string GetModalTitle() => _movieDetailResponse?.Title ?? "Movie trailer";

    private string GetPosterUriString(string posterPath) =>
        TMDBClient.GetPosterUri(posterPath).ToString();

    private string GetProfileUriString(string? profilePath) =>
        TMDBClient.GetProfileUri(profilePath).ToString();

    private bool HasTrailer() => _trailer is not null && !string.IsNullOrEmpty(_trailer.Key);

    private async Task PlayTrailerAsync()
    {
        var jsModule = await JsRuntime.InvokeAsync<IJSObjectReference>(
            "import",
            "./Components/Pages/MovieDetails.razor.js"
        );

        await using (jsModule)
        {
            if (HasTrailer())
            {
                var ytTrailerUrl = $"https://www.youtube.com/embed/{_trailer!.Key}";
                await jsModule.InvokeVoidAsync("initVideoPlayer", ytTrailerUrl);
            }
            else
            {
                await jsModule.InvokeVoidAsync("initVideoPlayer", string.Empty);
            }
        }
    }

    private void UpdateImageSources()
    {
        var newBackdropSrc = GetBackdropUriString(_movieDetailResponse!.BackdropPath);
        var newPosterSrc = GetPosterUriString(_movieDetailResponse.PosterPath);

        if (!string.Equals(newBackdropSrc, _backdropSrc, StringComparison.Ordinal))
        {
            _backdropSrc = newBackdropSrc;
            _isBackdropLoading = true;
        }

        if (!string.Equals(newPosterSrc, _posterSrc, StringComparison.Ordinal))
        {
            _posterSrc = newPosterSrc;
            _isPosterLoading = true;
        }
    }
}
