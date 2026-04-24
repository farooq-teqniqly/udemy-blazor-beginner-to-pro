## Why

On slow or flaky networks, movie poster images on the Now Playing page appear blank for several seconds before the image bytes arrive from TMDB's image CDN. The surrounding card chrome (title, release date, buttons) renders immediately from the movie list JSON, leaving a noticeable empty rectangle where the poster should be. Users have reported this as a "the page looks broken" moment. Showing a lightweight spinner inside each `MovieCard` while its poster is downloading gives clear feedback that the app is working and the image is on its way.

## What Changes

- Add a visible loading indicator to `MovieCard` that is shown from the moment the card renders until the poster image finishes downloading (or errors).
- Track per-card image load state in `MovieCard` and toggle the spinner off when the underlying `<img>` fires its `load` or `error` event.
- When the image fails to load, fall back to the existing `wwwroot/images/poster.png` placeholder that `TMDBClient.GetPosterUri` already returns for movies without a `PosterPath`, and hide the spinner so the card does not spin forever.
- Add a new `LoadingSpinner` UI component under `Components/UI` so the same indicator can be reused elsewhere (for example, future page-level loading states on `NowPlayingPage`, `Popular`, and `Favorites`).
- Introduce a component-scoped CSS rule set in `MovieCard.razor.css` that overlays the spinner on top of the poster area and reserves the poster's aspect ratio so the card layout does not shift when the image arrives.
- Add xUnit coverage in `NowPlayingApp.Tests` for the new `LoadingSpinner` component's parameters (for example, `Label`, `SizeRem`) and for `MovieCard`'s image load state transitions using `bUnit`-style render assertions if available, or via plain component state tests otherwise.

## Capabilities

### New Capabilities

- `movie-poster-display`: How movie poster images are rendered inside `MovieCard`, including the loading, loaded, and error states, the aspect-ratio-preserving layout, and the fallback to the local placeholder image.
- `ui-loading-spinner`: A reusable, presentational `LoadingSpinner` component in `Components/UI` that renders an accessible Bootstrap-styled spinner with configurable size and label, usable anywhere in the app.

### Modified Capabilities

- None. No existing `openspec/specs/` capabilities are in place yet, and no previously specified requirements are changing.

## Impact

- Affected code:
  - `UdemyBlazorBeginnerToPro/NowPlayingApp/Components/UI/MovieCard.razor` and `MovieCard.razor.css` (new image load state, spinner overlay, CSS for aspect ratio).
  - `UdemyBlazorBeginnerToPro/NowPlayingApp/Components/UI/LoadingSpinner.razor` and `LoadingSpinner.razor.css` (new files).
  - `UdemyBlazorBeginnerToPro/NowPlayingApp/_Imports.razor` already covers the `NowPlayingApp.Components.UI` namespace, so no change is expected there; a task will confirm.
  - `UdemyBlazorBeginnerToPro/NowPlayingApp.Tests` (new `LoadingSpinnerTests` and `MovieCardTests` classes).
- Affected APIs: None. No changes to `TMDBClient`, `MovieResponse`, `MovieListResponse`, or any HTTP contract.
- Dependencies: No new NuGet packages are expected. Bootstrap 5.3's existing `.spinner-border` styles are reused. If component rendering tests require `bUnit`, that will be called out explicitly in `design.md` before adding the package.
- Configuration and secrets: No new TMDB endpoints, no new `TMDBClientSettings` properties, no new `wwwroot/appsettings.json` keys, no new user-secrets. The existing TMDB image CDN and fallback asset `wwwroot/images/poster.png` are reused.

## Non-goals

- Changing how `NowPlayingPage` fetches movies or how the page-level "Loading Now Playing movies..." message works. That behavior stays as is for this change.
- Skeleton placeholders with shimmer animations, progressive/blurred-thumbnail loading, or responsive `srcset` selection. Those are larger UX efforts and are deliberately out of scope here.
- Any changes to sibling projects in the repository. The following are explicitly out of scope for this change:
  - `UdemyBlazorBeginnerToPro/BlazorLayout`
  - `UdemyBlazorBeginnerToPro/Fizzbuzz` and `UdemyBlazorBeginnerToPro/Fizzbuzz.Tests`
  - `UdemyBlazorBeginnerToPro/LoanShark` and `UdemyBlazorBeginnerToPro/LoanShark.Tests`
  - `UdemyBlazorBeginnerToPro/Tasker`
  - `UdemyBlazorBeginnerToPro/templates`
