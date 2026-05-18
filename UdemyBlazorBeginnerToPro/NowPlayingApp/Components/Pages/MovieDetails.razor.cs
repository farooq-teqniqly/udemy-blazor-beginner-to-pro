using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using NowPlayingApp.Models;
using NowPlayingApp.Services;

namespace NowPlayingApp.Components.Pages;

public partial class MovieDetails : IDisposable
{
    private string _backdropSrc = string.Empty;
    private CancellationTokenSource? _cancellationTokenSource;
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
        _isPageLoading = true;

        try
        {
            _movieDetailResponse = await TMDBClient.GetMovieDetail(
                MovieId,
                _cancellationTokenSource.Token
            );

            var newBackdropSrc = GetBackdropUriString(_movieDetailResponse.BackdropPath);
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
        catch (OperationCanceledException)
        {
            Logger.LogDebug($"{nameof(TMDBClient.GetMovieDetail)} request was cancelled.");
        }
        catch (HttpRequestException httpRequestException)
        {
            Logger.LogError(
                httpRequestException,
                $"{nameof(TMDBClient.GetMovieDetail)} - an error occurred."
            );
        }
        finally
        {
            _isPageLoading = false;
        }

        try
        {
            _trailer = await TMDBClient.GetTrailer(MovieId, _cancellationTokenSource.Token);
        }
        catch (OperationCanceledException)
        {
            Logger.LogDebug($"{nameof(TMDBClient.GetTrailer)} request was cancelled.");
        }
        catch (HttpRequestException httpRequestException)
        {
            Logger.LogError(
                httpRequestException,
                $"{nameof(TMDBClient.GetTrailer)} - an error occurred."
            );
        }
    }

    private string GetBackdropUriString(string backdropPath) =>
        TMDBClient.GetBackdropUri(backdropPath).ToString();

    private string GetModalTitle() => _movieDetailResponse?.Title ?? "Movie trailer";

    private string GetPosterUriString(string posterPath) =>
        TMDBClient.GetPosterUri(posterPath).ToString();

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
}
