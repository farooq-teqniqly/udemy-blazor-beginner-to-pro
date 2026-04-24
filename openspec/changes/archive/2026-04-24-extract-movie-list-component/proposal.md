## Why

`Popular.razor` and `NowPlayingPage.razor` render the same movie grid with the same loading and failure messages, and each owns its own copy of the TMDB fetch boilerplate (`CancellationTokenSource`, `try/catch/finally`, `ILogger` usage, and `Dispose`). Any fix to the loading UI, grid breakpoints, or cancellation handling has to be made twice and can easily drift between the two pages. Extracting the grid and its loading states into a shared `MovieList` component removes that duplication so each page only describes which TMDB call to make.

## What Changes

- Add a new `MovieList` Razor component under `Components/UI` that takes a `MovieListResponse?` and renders the existing `row g-3 row-cols-1 row-cols-sm-2 row-cols-lg-3 row-cols-xl-4` grid of `MovieCard` elements.
- Move the loading placeholder ("Loading ... movies...") and failure placeholder ("Failed to load ... movies.") markup out of `Popular.razor` and `NowPlayingPage.razor` into `MovieList`, keeping the page title phrase configurable through a `CategoryLabel` parameter (for example, `"Popular"` or `"Now Playing"`).
- Update `Popular.razor` and `NowPlayingPage.razor` to render `<MovieList Movies="..." IsLoading="..." CategoryLabel="..." />` instead of the inline `@if/else` and `@foreach` blocks.
- Keep each page responsible for its own TMDB fetch, `CancellationTokenSource` lifecycle, logging, and `Dispose`. The extraction is UI-only and does not move HTTP or cancellation concerns into `MovieList`.
- Add xUnit coverage in `NowPlayingApp.Tests` for `MovieList`'s parameter contract (`Movies`, `IsLoading`, `CategoryLabel`) and for the three render states: loading, error, and populated.

## Capabilities

### New Capabilities

- `movie-list-grid`: A reusable movie grid component that renders a responsive Bootstrap grid of `MovieCard` elements for a `MovieListResponse`, including the loading and failure placeholder states and an accessible label that identifies which movie category is loading or failed.

### Modified Capabilities

- None. No existing `openspec/specs/` capabilities describe the page-level movie grid today; `movie-poster-display` and `ui-loading-spinner` remain unchanged.

## Impact

- Affected code:
  - `UdemyBlazorBeginnerToPro/NowPlayingApp/Components/UI/MovieList.razor` and `MovieList.razor.css` (new files).
  - `UdemyBlazorBeginnerToPro/NowPlayingApp/Components/Pages/Popular.razor` (replace inline grid with `<MovieList />`).
  - `UdemyBlazorBeginnerToPro/NowPlayingApp/Components/Pages/NowPlayingPage.razor` (replace inline grid with `<MovieList />`).
  - `UdemyBlazorBeginnerToPro/NowPlayingApp/_Imports.razor` already exposes `NowPlayingApp.Components.UI`, so no change is expected there; a task will confirm.
  - `UdemyBlazorBeginnerToPro/NowPlayingApp.Tests` (new `MovieListTests` class).
- Affected APIs: None. No changes to `TMDBClient`, `MovieResponse`, `MovieListResponse`, or any HTTP contract.
- Dependencies: No new NuGet packages. Existing Bootstrap 5.3 grid and icon classes are reused.
- Configuration and secrets: No new TMDB endpoints, no new `TMDBClientSettings` properties, no new `wwwroot/appsettings.json` keys, no new user-secrets entries, and no new Bruno requests.

## Non-goals

- Unifying the page-level fetch/cancellation/logging pattern into a shared base class, service, or hook. Each page keeps its own `@code` block for this change.
- Replacing the text-based loading placeholder with the `LoadingSpinner` component or any other visual spinner. The existing `<h2>Loading ... movies...</h2>` text is preserved verbatim inside `MovieList`.
- Changing `MovieCard` internals, `MovieCard.razor.css`, or how posters load. `movie-poster-display` and `ui-loading-spinner` behavior is untouched.
- Adding paging, infinite scroll, sorting, filtering, or any new interactive behavior to the grid.
- Changes to the `Favorites` or `Home` pages, even if they could later adopt `MovieList`. Adopting `MovieList` elsewhere is a follow-up change.
- Any changes to sibling projects in the repository. The following are explicitly out of scope for this change:
  - `UdemyBlazorBeginnerToPro/BlazorLayout`
  - `UdemyBlazorBeginnerToPro/Fizzbuzz` and `UdemyBlazorBeginnerToPro/Fizzbuzz.Tests`
  - `UdemyBlazorBeginnerToPro/LoanShark` and `UdemyBlazorBeginnerToPro/LoanShark.Tests`
  - `UdemyBlazorBeginnerToPro/Tasker`
  - `UdemyBlazorBeginnerToPro/templates`
