## Context

`Popular.razor` and `NowPlayingPage.razor` are two near-identical Blazor WebAssembly pages under `Components/Pages`. Each page:

- Injects `TMDBClient` and `ILogger<NowPlayingPage>` (today `Popular.razor` reuses the `NowPlayingPage` logger type, which is a separate bug to flag but not fix here).
- Owns a `CancellationTokenSource? _cancellationTokenSource`, a `MovieListResponse?` backing field, and a `bool _isLoading` flag.
- In `OnInitializedAsync`, creates the token source, flips `_isLoading = true`, calls a specific `TMDBClient` method (`GetPopularMovies` or `GetNowPlayingMovies`), and catches `OperationCanceledException` and `HttpRequestException`.
- In `Dispose`, cancels and disposes the token source.
- Renders a Bootstrap grid (`row g-3 row-cols-1 row-cols-sm-2 row-cols-lg-3 row-cols-xl-4`) that either shows a centered `<h2>` loading or failure message, or iterates `Results` and renders a `MovieCard` per movie.

The grid markup and the loading/failure messages are duplicated almost verbatim. The only differences are the category label ("Popular" vs "Now Playing") and which TMDB endpoint is invoked. This change pulls the shared UI into a single component so future tweaks to the grid breakpoints, accessibility attributes, or loading messages happen in one place.

Relevant existing surfaces:

- `Components/UI/MovieCard.razor` remains the unit the grid iterates over.
- `Components/UI/LoadingSpinner.razor` exists but is intentionally not adopted here; the text-based loading message matches the current behavior.
- `_Imports.razor` already brings `NowPlayingApp.Components.UI` and `NowPlayingApp.Models` into scope for all components.
- `Models/MovieListResponse` exposes a `Results` collection of `MovieResponse`.

## Goals / Non-Goals

**Goals:**

- Eliminate the duplicated grid markup and loading/failure placeholders between `Popular.razor` and `NowPlayingPage.razor`.
- Introduce one new component, `Components/UI/MovieList.razor`, with a small, explicit parameter surface: `Movies`, `IsLoading`, and `CategoryLabel`.
- Preserve the current user-visible output byte-for-byte where feasible (same Bootstrap classes, same text content, same heading level).
- Keep TMDB fetching, cancellation, and logging concerns on the page components where they already live.
- Establish unit-test coverage for the new component's parameter contract and render states.

**Non-Goals:**

- Centralizing the TMDB fetch/cancel/log pattern into a base class, service, or hook. That is a larger refactor and is deliberately deferred.
- Swapping the text "Loading ... movies..." for a visual spinner. `LoadingSpinner` adoption is out of scope here.
- Changing `MovieCard`, its poster loading behavior, or `movie-poster-display` semantics.
- Adopting `MovieList` on `Favorites.razor` or any future page in this change. That is a follow-up.
- Any changes to sibling projects (`BlazorLayout`, `Fizzbuzz`, `Fizzbuzz.Tests`, `LoanShark`, `LoanShark.Tests`, `Tasker`, `templates`).

## Decisions

### Decision 1: Add `MovieList` under `Components/UI` and drive it with three `[Parameter]` properties

`MovieList` will live at `UdemyBlazorBeginnerToPro/NowPlayingApp/Components/UI/MovieList.razor`. It exposes three public parameters:

- `MovieListResponse? Movies { get; set; }` - the fetched response; `null` when the fetch has not completed or failed.
- `bool IsLoading { get; set; }` - indicates whether a fetch is currently in flight. When `Movies` is `null`, the component renders either the loading or failure placeholder based on this flag.
- `string CategoryLabel { get; set; } = "";` - the human-readable category used to compose the placeholder text (for example, `"Popular"` yields "Loading Popular movies..." and "Failed to load Popular movies.").

Rationale:

- The current pages express their render state as `(Movies is null, IsLoading)`; mirroring that tuple in the parameter surface keeps the move straightforward and minimizes semantic changes.
- A single `CategoryLabel` string covers both the loading and failure messages without needing `RenderFragment` parameters, matching the text formats already in `Popular.razor` and `NowPlayingPage.razor`.
- Defaulting `CategoryLabel` to an empty string preserves the render contract under C# nullable reference types and avoids forcing callers to pass the value in tests where they do not care (for example, assertions that focus on `IsLoading`).

Alternatives considered:

- Exposing a single `RenderState` enum with values like `Loading`, `Error`, `Loaded`. Rejected as over-engineered for two booleans; it would also require the pages to map their existing `(Movies, IsLoading)` state into the enum, adding rather than removing code.
- Using `RenderFragment? LoadingTemplate` and `RenderFragment? ErrorTemplate` parameters to let each page supply its own placeholders. Rejected because both pages use the same placeholder markup today; templating is only useful when the variation exists.
- Letting `MovieList` own the TMDB call via an `EventCallback<CancellationToken, Task<MovieListResponse?>> Loader` parameter. Rejected because it merges data-fetching with presentation, and the proposal's non-goals explicitly keep fetching on the pages.

### Decision 2: Render the exact Bootstrap grid and placeholder markup the pages use today

The component body will reproduce the existing markup verbatim:

```html
<div class="row g-3 row-cols-1 row-cols-sm-2 row-cols-lg-3 row-cols-xl-4">
    @if (Movies is null)
    {
        <div class="d-flex justify-content-center align-items-center">
            @if (IsLoading)
            {
                <h2>Loading @CategoryLabel movies...</h2>
            }
            else
            {
                <h2 class="text-muted">Failed to load @CategoryLabel movies.</h2>
            }
        </div>
    }
    else
    {
        @foreach (var movie in Movies.Results)
        {
            <div class="col">
                <MovieCard Movie="movie"></MovieCard>
            </div>
        }
    }
</div>
```

Rationale:

- The grid and placeholder copy are already identical across both pages; reusing them means the behavior change for users is zero.
- Preserving the same Bootstrap classes means no styling review is needed; `app.css`/`themes.css` rules and the responsive breakpoints continue to apply.
- Using `@CategoryLabel` for the placeholder text keeps the phrase structure identical to today's hard-coded "Popular" / "Now Playing" strings.

Alternatives considered:

- Dropping the outer `<div class="row ...">` and letting callers wrap `<MovieList />` in their own row. Rejected because it would re-introduce boilerplate on each page and defeat the purpose of the extraction.
- Replacing the `<h2>` placeholders with a `LoadingSpinner` or an icon for failure. Rejected per the non-goals; keeping a faithful lift-and-shift minimizes the risk surface for this change.

### Decision 3: Pages keep owning the fetch lifecycle and pass state into `MovieList`

`Popular.razor` and `NowPlayingPage.razor` keep their `_cancellationTokenSource`, `_isLoading`, and response fields, their `OnInitializedAsync` fetch, their exception handling, and their `Dispose`. Their markup shrinks to something like:

```razor
<PageTitle>Popular</PageTitle>
<div>
    <h1>
        <i class="bi bi-stars text-warning"></i>
        Popular Movies
    </h1>
    <p class="lead italic">Movies that are currently popular with TMDB users.</p>
    <MovieList Movies="_popularMovies" IsLoading="_isLoading" CategoryLabel="Popular" />
</div>
```

Rationale:

- The proposal's non-goals keep fetching, cancellation, and logging on the pages; this decision is an explicit expression of that.
- Page-level concerns (page title, header icon, lead paragraph, route) are inherently per-page, so they stay where they are.
- Keeping `TMDBClient` and `ILogger<T>` injected only on the pages avoids broadening the `MovieList` component's dependencies; `MovieList` remains a presentation-only component with no `@inject` directives.

Alternatives considered:

- Moving the fetch lifecycle into a new `MovieListHost` component and relegating the page to just picking the category. Rejected as a larger refactor outside this proposal's scope; worth revisiting only if a third page is added.
- Giving `MovieList` a cascading `ILogger<MovieList>` and logging render decisions. Rejected; there is no observable benefit and it adds noise to logs.

### Decision 4: Fix the `ILogger<NowPlayingPage>` typo in `Popular.razor` opportunistically

`Popular.razor` currently injects `ILogger<NowPlayingPage>` even though it is not the `NowPlayingPage`. While replacing the inline grid with `<MovieList />`, change the injection to `ILogger<Popular>` so future log filtering works correctly per page. This is a one-line change inside a file being modified anyway and carries no risk beyond correcting category names in log output.

Rationale:

- It is a trivial, in-scope correction to a file the change already edits.
- Keeping the bug in place would be a deliberate choice to preserve a defect; calling it out as a decision documents the fix so the reviewer does not wonder why the logger type changed.

Alternatives considered:

- Leaving `ILogger<NowPlayingPage>` on `Popular.razor` and fixing it in a later change. Rejected; no reason to split a trivial, related fix into a separate proposal.

### Decision 5: Test `MovieList` via direct component instantiation, matching the existing test style

`LoadingSpinnerTests` in the archived `add-movie-poster-loading-spinner` change established the pattern of instantiating a Razor component's generated class directly and asserting on `[Parameter]` defaults and setter round-trips. `MovieListTests` will follow the same pattern:

- Default values: `IsLoading` defaults to `false`, `Movies` defaults to `null`, `CategoryLabel` defaults to `""`.
- Setter round-trips: setting each parameter and reading it back returns the assigned value.
- Because the render branches are driven entirely by parameters (no internal state beyond what Blazor wires up), the unit tests focus on the parameter contract. A follow-up change can add `bUnit` if behavior verification on rendered HTML becomes necessary across more components.

Rationale:

- Matches the existing test style already in the repository.
- Avoids adding `bUnit` as a new dependency just for three simple `if/else` branches.
- Keeps the tests fast and deterministic, aligned with the rest of `NowPlayingApp.Tests`.

Alternatives considered:

- Introducing `bUnit` to assert on rendered DOM. Deferred; cost and added dependency do not yet justify the benefit for a three-parameter component.
- Refactoring render branches into an `internal` method that returns an enum and testing that. Rejected; Razor components do not benefit from that indirection, and it would complicate the component for no test-coverage gain beyond what parameter tests already give.

### Decision 6: No DI, configuration, or Bruno changes

`MovieList` is a presentation-only component. `Program.cs` stays as is, `AddStandardResilienceHandler` continues to apply only to `TMDBClient`, `TMDBClientSettings` is unchanged, there are no new `wwwroot/appsettings.json` keys, no new user-secrets entries, no new TMDB endpoints, and no new or modified Bruno requests under `NowPlayingApp/BrunoTests/TMDB`.

## Risks / Trade-offs

- Risk: Component-scoped CSS in `MovieList.razor.css` (if added) could shadow the grid styles currently controlled by Bootstrap. Mitigation: start with no `MovieList.razor.css` file; if one is added later, limit it to container-level concerns and rely on Bootstrap utility classes for the grid.
- Risk: Moving the `<h2>` placeholders inside `MovieList` changes the DOM parent of the loading/failure text from the page's `<div>` to the component's `<div class="row ...">`. Mitigation: the visual output is unchanged because there are no selectors targeting that specific parent; a quick manual smoke test on `/popular` and `/now-playing` confirms.
- Risk: `ILogger<Popular>` as a new logger category in `Popular.razor` changes the category name observed in logs for that page (previously `NowPlayingPage`). Mitigation: documented as Decision 4; since this is local-development coursework, no external log filters depend on the old category name.
- Trade-off: Accepting two-parameter state (`Movies`, `IsLoading`) instead of a dedicated enum. This is consistent with the existing `(null, bool)` state the pages already juggle and keeps the migration a pure lift-and-shift.
- Trade-off: No rendered-HTML tests for the component. Parameter-contract tests still catch accidental breakage of the public surface; any follow-up change that needs richer UI testing can add `bUnit` at that time.

## Migration Plan

This is an additive, client-side UI change with no data model, API, or configuration impact. Deployment follows the normal Blazor WebAssembly publish/deploy cycle: build, publish the static assets, and serve. No feature flag is needed.

Rollback: revert the single commit containing the change. The only files touched are the two pages, the new `MovieList` component (and optional companion CSS), `NowPlayingApp.Tests` additions, and, if confirmed necessary, `_Imports.razor`. There is no database migration, no cached data, and no configuration state to unwind.

## Open Questions

- Should `Favorites.razor` also adopt `<MovieList />` as part of this change? Proposed answer: no. `Favorites.razor` renders a different data source and its behavior is not duplicated with `Popular` / `NowPlayingPage` today; adopting `MovieList` there can land in a follow-up once the `MovieList` contract is observed in production.
- Should `MovieList` expose an `EmptyLabel` parameter for the case where `Movies.Results` is empty (for example, TMDB returns zero results on a narrow date window)? Proposed answer: defer. Today's pages do not handle that case either; when a spec for empty results is defined, extend `MovieList` then.
