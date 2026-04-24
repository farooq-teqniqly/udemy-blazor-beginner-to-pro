## Context

The Now Playing page currently renders each movie as a `MovieCard` that immediately includes an `<img src="...">` pointing at the TMDB image CDN (`TMDBClient.GetPosterUri(movie.PosterPath)`). On slow networks the JSON payload for `GetNowPlayingMovies` arrives relatively quickly, but the remote poster images can take seconds to download. During that window, the card shows empty whitespace where the poster should be, because nothing visible fills the `<img>` element until the browser has pixels to paint.

The existing page-level loading message in `NowPlayingPage.razor` ("Loading Now Playing movies...") only covers the JSON fetch, not individual image loads, so it disappears before the posters are usable. This change introduces a per-image loading state inside `MovieCard` itself so the poster area always has meaningful feedback.

Relevant existing surfaces:

- `Components/UI/MovieCard.razor` and `MovieCard.razor.css` own the card markup and hover transform.
- `Services/TMDBClient.GetPosterUri(string posterPath)` returns either the TMDB image URL or the local `/images/poster.png` fallback when `posterPath` is null or empty.
- `wwwroot/images/poster.png` is the existing local fallback asset.
- `_Imports.razor` already exposes `NowPlayingApp.Components.UI`, so any new component in `Components/UI` is available without additional `@using` lines.

## Goals / Non-Goals

Goals:

- Always show a visible spinner inside each `MovieCard` poster area from the moment the card mounts until its poster image has either loaded or errored.
- Reserve the poster's footprint (aspect ratio and width) so there is no visible layout shift when the image appears.
- Provide a reusable `LoadingSpinner` component under `Components/UI` so the same indicator can be reused for future page-level and inline loading states.
- Fail gracefully: on image `error`, hide the spinner and fall back to the local `poster.png` placeholder so no card spins forever.
- Keep the implementation dependency-free: reuse Bootstrap 5.3's `spinner-border` styles and component-scoped CSS. No new NuGet packages.

Non-Goals:

- Rewriting `NowPlayingPage.razor`'s page-level loading behavior or adding a spinner there. The current text-based message stays for now.
- Skeleton shimmer placeholders, blurred-thumbnail progressive loading, or responsive `srcset` selection. These are larger UX investments.
- Preloading or prefetching images, or caching remote posters in local storage.
- Any changes to `TMDBClient`, its settings, `MovieResponse`, or `MovieListResponse`. This is a pure UI/presentation change.
- Changes to any sibling project (`BlazorLayout`, `Fizzbuzz`, `Fizzbuzz.Tests`, `LoanShark`, `LoanShark.Tests`, `Tasker`, `templates`).

## Decisions

### Decision 1: Track image load state with the native `<img>` `load` and `error` events via Blazor event binding

The `MovieCard` component will keep a single private `bool _isPosterLoading` field, initialized to `true`, and bind the `<img>` element's `@onload` and `@onerror` events to a handler that sets `_isPosterLoading = false` and then calls `StateHasChanged` (implicitly, since it is an `EventCallback` in a Razor `@code` block). The spinner is rendered with `@if (_isPosterLoading)` overlayed on top of the image.

Rationale:

- Blazor WebAssembly exposes both events as standard DOM events that can be wired with `@onload` and `@onerror` directly from Razor, avoiding any need for `IJSRuntime` interop.
- This keeps state local to the component and in C# - no JS files, no bootstrap events on window, no timers.
- It works uniformly for cached and non-cached images. The `load` event fires in both cases, so cards do not flash a spinner unnecessarily when images are already in the browser cache on subsequent visits.

Alternatives considered:

- CSS-only approach with `background-image` on a `<div>` and a spinner underneath via `z-index`. This works but gives us no clean way to detect the error state or to remove the spinner, so cards with broken remote images would spin forever.
- JS interop to preload images with `new Image()` and swap on `onload`. More moving parts for no extra benefit when Blazor's native event binding already handles it.
- Using `loading="lazy"` on the `<img>`. This helps off-screen images but does not solve the "image is loading and I want a spinner" problem, and it fights with the reserved-aspect-ratio approach.

### Decision 2: Overlay the spinner on top of the `<img>` using a wrapper `<div>` with `position: relative`

The current `<img class="card-img-top">` will be wrapped in a new `<div class="movie-card-poster">` that is `position: relative` and enforces an aspect ratio matching TMDB's w500 posters (2:3). The `<img>` inside becomes `position: absolute` filling the wrapper, and the `LoadingSpinner` is rendered as a sibling absolutely positioned to the center. While loading, the `<img>` has `visibility: hidden` so it does not compete visually with the spinner but still contributes to the layout reservation (actually the wrapper's `aspect-ratio` handles that; `visibility: hidden` just hides the partially-painted decode).

Rationale:

- Reserving the poster's aspect ratio in a wrapper keeps the whole grid from reflowing as each card's poster finishes loading.
- Using Bootstrap classes where possible (`d-flex justify-content-center align-items-center` on the wrapper) minimizes custom CSS.
- Scoping the new styles to `MovieCard.razor.css` keeps them from leaking into other cards or future UI.

Alternatives considered:

- Setting explicit pixel dimensions on the `<img>` via inline style. Rejected because it ignores the responsive grid (`row-cols-1 row-cols-sm-2 row-cols-lg-3 row-cols-xl-4`); widths differ across breakpoints.
- Keeping the `<img>` visible during loading and placing the spinner over a half-painted image. Rejected for visual clarity - it is jarring to see a spinner stacked over a decoded-partway image.

### Decision 3: Introduce `Components/UI/LoadingSpinner.razor` instead of inlining the markup

A new `LoadingSpinner` Razor component will live at `UdemyBlazorBeginnerToPro/NowPlayingApp/Components/UI/LoadingSpinner.razor`, with component-scoped CSS at `LoadingSpinner.razor.css`. It exposes two `[Parameter]` public properties:

- `string Label { get; set; } = "Loading...";` - drives the visually-hidden status text.
- `double SizeRem { get; set; } = 2;` - controls the spinner's diameter via an inline `style` attribute.

The component renders:

```html
<div class="spinner-border text-primary" role="status" style="width: @(SizeRem)rem; height: @(SizeRem)rem;">
    <span class="visually-hidden">@Label</span>
</div>
```

Rationale:

- One reusable component with two parameters satisfies both `MovieCard`'s needs and any future page-level use (for example, replacing the text-only loading state in `NowPlayingPage.razor` in a later change).
- Parameters default to sensible values, so parameterless usage just works.
- Using Bootstrap's `spinner-border` and `visually-hidden` classes keeps accessibility and styling consistent with the rest of the app, and requires no new CSS beyond an optional inline size override.

Alternatives considered:

- Inlining the spinner markup directly in `MovieCard`. Rejected because reusability was called out in the proposal and keeping markup local would duplicate `role="status"` and `visually-hidden` patterns across future components.
- Passing child content with `RenderFragment ChildContent` for the label. Rejected as overkill; plain text via a `string Label` parameter is simpler and covers the known use cases.

### Decision 4: Testing strategy uses plain xUnit tests on component parameters plus one integration-style manual check

Because the repository's current test setup uses xUnit 2.9 + NSubstitute and does not include `bUnit`, adding a bUnit dependency just for two small components is not justified for this change. The test coverage plan is:

- For `LoadingSpinner`, write unit tests by instantiating the component class directly and asserting on its public parameter defaults (for example, `Label` defaults to `"Loading..."` and `SizeRem` defaults to `2`). This gives a failure-path signal if someone accidentally changes the contract.
- For `MovieCard`'s image load state, write a small unit test against the `MovieCard` code-behind fields if they are made `internal` with `InternalsVisibleTo("NowPlayingApp.Tests")`, or refactor the load state into a tiny, test-friendly helper method (for example, a `SetPosterLoaded()` method) that flips the flag. This allows `MovieCardTests` to assert that `_isPosterLoading` transitions correctly without needing a render host.
- Manual smoke test: launch the app with network throttling set to "Slow 3G" in the browser dev tools and confirm the spinner shows for each card until the poster arrives.

Rationale:

- Keeps the test project dependency surface unchanged, matching the project's "no new packages unless justified" stance.
- `[Parameter]`-validation tests and simple state-flag tests still catch regressions in the component contract, which is what the specs assert.
- If a future change broadens the UI test surface, `bUnit` can be added at that point without blocking this change.

Alternatives considered:

- Adding `bUnit` (`Microsoft.AspNetCore.Components.Testing` ecosystem). Defer. Documented as a follow-up if UI testing becomes a recurring need.
- Playwright/end-to-end tests driven from CI. Out of scope and disproportionate for a spinner.

### Decision 5: No DI registration changes, no new configuration keys

`LoadingSpinner` is a presentation-only Razor component with no services or state to register, so `Program.cs` is unchanged. `AddStandardResilienceHandler` on the typed `TMDBClient` is also unaffected. No new `TMDBClientSettings` properties, no new `wwwroot/appsettings.json` keys, no new user-secrets entries, and no new TMDB endpoint or Bruno request. This change is strictly a client-side UI enhancement.

## Risks / Trade-offs

- Risk: The `<img>` `load` event might fire before Blazor has attached the event binding on very fast connections (cached images), leaving a brief spinner flash that then immediately disappears. Mitigation: the flash is harmless (< 1 frame) and is preferable to the alternative of missing load events; empirically Blazor's event binding happens synchronously during render, so cached images will still trigger `@onload` on first paint.
- Risk: If the TMDB image CDN returns a non-200 for a valid-looking URL (for example, rate limiting), the card falls back to `poster.png` but the movie is effectively posterless visually. Mitigation: accepted trade-off; it is the existing behavior when `PosterPath` is empty and keeps the UI consistent.
- Trade-off: Wrapping `<img>` in a `position: relative` container slightly changes the DOM structure for `MovieCard`. Any component-scoped CSS that was targeting the old top-level `<img>` directly (there is none today) would need updating. Mitigation: the only consumer of these styles is `MovieCard.razor.css`, and it is updated as part of the change.
- Trade-off: Skipping `bUnit` means component rendering is not tested end-to-end. Mitigation: the public parameter surface is small (two properties on `LoadingSpinner`) and can be exercised via direct instantiation; richer UI testing can be introduced later when the app has more interactive components to justify the dependency.

## Migration Plan

This is an additive, client-side UI change with no data model, API, or configuration impact. Deployment is the normal publish/deploy cycle for the Blazor WebAssembly app: build, publish the static assets, and serve. No feature flag is needed.

Rollback: revert the single commit containing the change. The only files touched are new or existing files inside `NowPlayingApp` and `NowPlayingApp.Tests`; there is no database migration, no cached data, and no configuration state to unwind.

## Open Questions

- Should `NowPlayingPage.razor`'s page-level loading state also adopt `LoadingSpinner` in this change for consistency? Proposed answer: no, defer to a follow-up change so this one stays focused on the poster image use case.
- Is a 2:3 aspect ratio on the poster wrapper acceptable for all TMDB image sizes, or should it be driven by a CSS variable so we can switch between w342/w500/w780 later? Proposed answer: hard-code 2:3 now (it matches TMDB's portrait poster art across sizes); revisit only when we add responsive `srcset`.
