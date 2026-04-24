## 1. Create the `MovieList` component

- [x] 1.1 Create `UdemyBlazorBeginnerToPro/NowPlayingApp/Components/UI/MovieList.razor` with three public `[Parameter]` properties: `MovieListResponse? Movies { get; set; }`, `bool IsLoading { get; set; }`, and `string CategoryLabel { get; set; } = "";`.
- [x] 1.2 Render the Bootstrap grid container `<div class="row g-3 row-cols-1 row-cols-sm-2 row-cols-lg-3 row-cols-xl-4">` as the single top-level element, matching the breakpoints currently in `Popular.razor` and `NowPlayingPage.razor`.
- [x] 1.3 Inside the grid, render the loading placeholder `<div class="d-flex justify-content-center align-items-center"><h2>Loading @CategoryLabel movies...</h2></div>` when `Movies is null && IsLoading`.
- [x] 1.4 Inside the grid, render the failure placeholder `<div class="d-flex justify-content-center align-items-center"><h2 class="text-muted">Failed to load @CategoryLabel movies.</h2></div>` when `Movies is null && !IsLoading`.
- [x] 1.5 Inside the grid, when `Movies` is non-null, iterate `Movies.Results` with `@foreach` and render `<div class="col"><MovieCard Movie="movie" /></div>` for each `MovieResponse`.
- [x] 1.6 Confirm `UdemyBlazorBeginnerToPro/NowPlayingApp/_Imports.razor` already exposes `NowPlayingApp.Components.UI`; leave unchanged, or add the namespace if it has been removed.
- [x] 1.7 Decide whether `MovieList.razor.css` is needed; if so, add only container-level rules that do not shadow Bootstrap grid styling. Skip the file if no component-scoped styles are required.

## 2. Adopt `MovieList` on `Popular.razor`

- [x] 2.1 In `UdemyBlazorBeginnerToPro/NowPlayingApp/Components/Pages/Popular.razor`, remove the inline `<div class="row g-3 row-cols-1 row-cols-sm-2 row-cols-lg-3 row-cols-xl-4">` block, including the `@if/else` placeholders and the `@foreach` over `_popularMovies.Results`.
- [x] 2.2 Replace that markup with `<MovieList Movies="_popularMovies" IsLoading="_isLoading" CategoryLabel="Popular" />` inside the existing `<div>` that holds the page heading and lead paragraph.
- [x] 2.3 Change the `@inject ILogger<NowPlayingPage> Logger` directive in `Popular.razor` to `@inject ILogger<Popular> Logger` so the logger category matches the page. Do not change the `catch` messages or `_cancellationTokenSource` lifecycle.
- [x] 2.4 Confirm the `@code` block still owns `_cancellationTokenSource`, `_popularMovies`, `_isLoading`, `OnInitializedAsync`, and `Dispose`, and that the `try/catch/finally` around `TMDBClient.GetPopularMovies` is unchanged other than possibly narrowing `catch` messages to mention `Popular` instead of `NowPlaying`.

## 3. Adopt `MovieList` on `NowPlayingPage.razor`

- [x] 3.1 In `UdemyBlazorBeginnerToPro/NowPlayingApp/Components/Pages/NowPlayingPage.razor`, remove the inline `<div class="row g-3 row-cols-1 row-cols-sm-2 row-cols-lg-3 row-cols-xl-4">` block, including the `@if/else` placeholders and the `@foreach` over `_nowPlayingMovies.Results`.
- [x] 3.2 Replace that markup with `<MovieList Movies="_nowPlayingMovies" IsLoading="_isLoading" CategoryLabel="Now Playing" />` inside the existing `<div>` that holds the page heading and lead paragraph.
- [x] 3.3 Leave the `@inject ILogger<NowPlayingPage> Logger` directive in place; this page is already typed correctly.
- [x] 3.4 Confirm the `@code` block still owns `_cancellationTokenSource`, `_nowPlayingMovies`, `_isLoading`, `OnInitializedAsync`, and `Dispose`, and that the `try/catch/finally` around `TMDBClient.GetNowPlayingMovies` is unchanged.

## 4. Tests for `MovieList`

- [x] 4.1 Add `UdemyBlazorBeginnerToPro/NowPlayingApp.Tests/Components/UI/MovieListTests.cs` with xUnit `[Fact]` tests that instantiate `MovieList` directly (no `TMDBClient` or `IOptions<T>` dependencies required since the component injects nothing).
- [x] 4.2 Add `Movies_When_DefaultConstructed_Returns_Null`, `IsLoading_When_DefaultConstructed_Returns_False`, and `CategoryLabel_When_DefaultConstructed_Returns_EmptyString` to assert the default parameter values named in the spec.
- [x] 4.3 Add `Movies_When_Set_Returns_AssignedInstance`, `IsLoading_When_Set_Returns_AssignedValue`, and `CategoryLabel_When_Set_Returns_AssignedValue` to exercise the parameter setter round-trip. For `Movies`, use a minimal `MovieListResponse` instance constructed directly without NSubstitute.

## 5. Verification

- [x] 5.1 Run `dotnet build UdemyBlazorBeginnerToPro/UdemyBlazorBeginnerToPro.slnx` and confirm the solution builds with zero new warnings introduced by this change.
- [x] 5.2 Run `dotnet test UdemyBlazorBeginnerToPro/NowPlayingApp.Tests/NowPlayingApp.Tests.csproj` and confirm the new `MovieListTests` pass together with the existing test suite.
- [ ] 5.3 Run `UdemyBlazorBeginnerToPro/NowPlayingApp` locally, navigate to `/popular` and `/now-playing`, and confirm each page still shows its heading, lead paragraph, loading text during the initial fetch, the expected card grid once data arrives, and no console errors.
- [x] 5.4 Confirm that no new `PackageReference` entries were added to `UdemyBlazorBeginnerToPro/NowPlayingApp/NowPlayingApp.csproj` or `UdemyBlazorBeginnerToPro/NowPlayingApp.Tests/NowPlayingApp.Tests.csproj`.
- [x] 5.5 Confirm there are no changes under `UdemyBlazorBeginnerToPro/NowPlayingApp/BrunoTests/TMDB/` because this change does not introduce or modify any TMDB endpoint consumption.
- [x] 5.6 Grep `Popular.razor` and `NowPlayingPage.razor` to confirm neither file still contains `row-cols-xl-4` or a `@foreach` over `MovieListResponse.Results`, proving the duplication was fully removed.
