## Context

`NowPlayingApp` already provides page-level patterns for loading TMDB-backed movie lists (`NowPlayingPage` and `Popular`) and rendering those lists through the shared `MovieList` and `MovieCard` UI components. The search feature extends that pattern to a query-driven flow initiated from `TopNavMenu` and fulfilled by TMDB's `search/movie` endpoint.

The implementation is constrained by the existing architecture:
- UI code lives in `Components/Layout` and `Components/Pages`.
- TMDB API calls are centralized in `Services/TMDBClient.cs` with a typed `HttpClient` registration in `Program.cs`.
- Async page loads use component-owned `CancellationTokenSource` instances that are cancelled and disposed in `Dispose`.
- No new secrets or environment-specific configuration should be introduced for this feature.

## Goals / Non-Goals

**Goals:**
- Provide a search entry point in `TopNavMenu` that navigates to `/search?q=<query>`.
- Add a `/search` page in `Components/Pages` that retrieves results via `TMDBClient.SearchMovies` and renders movies with `MovieList`.
- Reuse the existing typed `HttpClient` registration for `TMDBClient` in `Program.cs`, including `AddStandardResilienceHandler`.
- Handle empty query input, request cancellation, and HTTP failures without crashing the page.
- Add automated tests in `NowPlayingApp.Tests` for the client method and page behavior.

**Non-Goals:**
- Adding new services or replacing `TMDBClient` with a separate search-specific client.
- Adding pagination, advanced filters, or query suggestion UX.
- Introducing new `TMDBClientSettings` keys, appsettings entries, or user-secrets values.
- Changing required/optional `[Parameter]` semantics on existing shared UI components.

## Decisions

- **Use `TopNavMenu` query-string navigation for search.**
  - Decision: Keep the search form in `Components/Layout/TopNavMenu.razor` and navigate with `NavigationManager.NavigateTo($"search?q={_query}")`.
  - Rationale: This preserves existing global navigation behavior and avoids adding duplicate search entry points.
  - Alternative considered: Add a dedicated home-page search section. Rejected to avoid duplicated UX and logic.

- **Implement TMDB search in `TMDBClient` as `SearchMovies`.**
  - Decision: Add `SearchMovies(string query, CancellationToken cancellationToken = default)` to `Services/TMDBClient.cs`.
  - Rationale: `TMDBClient` already encapsulates TMDB endpoint construction, HTTP execution, and deserialization, so search fits naturally in the same abstraction.
  - Alternative considered: New `SearchService` wrapper over `HttpClient`. Rejected because it would duplicate request and error handling patterns already in `TMDBClient`.

- **Use a dedicated search page component with query binding.**
  - Decision: Add `Components/Pages/Search.razor` with `[SupplyParameterFromQuery(Name = "q")]` for query input and code-behind state management in `Search.razor.cs`.
  - Rationale: Page-level state aligns with existing page components, and query binding enables shareable URLs.
  - Alternative considered: Render search results inside layout or nav. Rejected due to lifecycle complexity and weaker routing semantics.

- **Do not modify DI registrations for this feature.**
  - Decision: Reuse the existing typed `TMDBClient` registration in `Program.cs`; no additional `AddHttpClient` registrations are required.
  - Rationale: `TMDBClient` already has token setup and resilience configured; search requests should inherit that behavior.
  - Alternative considered: Separate typed client for search endpoint. Rejected as unnecessary for current scope.

- **Parameter requirements remain unchanged for shared components.**
  - Decision: Continue using `MovieList` with its existing required parameters (`[Parameter, EditorRequired]` for `Movies`, `IsLoading`, and `CategoryLabel`).
  - Rationale: Search results are another movie-list source and should conform to the same component contract.
  - Alternative considered: Add search-specific list component with optional parameters. Rejected to avoid unnecessary UI divergence.

## Risks / Trade-offs

- [Risk] Query strings with special characters may not be URL-encoded before API request construction - this can lead to malformed search URLs for some terms.  
  Mitigation: Keep tests focused on current behavior and plan a follow-up enhancement to encode query values explicitly.

- [Risk] No pagination means only the first TMDB results page is visible.  
  Mitigation: Document pagination as a non-goal and keep endpoint request fixed to `page=1`.

- [Risk] Search page currently treats empty results and failed loads similarly in the rendered message.  
  Mitigation: Log HTTP failures and preserve stable user-facing behavior; defer nuanced UX differentiation to a later change.

- [Trade-off] Keeping search inside `TMDBClient` increases that class surface area.  
  Mitigation: Continue using method-level segmentation (`GetNowPlayingMovies`, `GetPopularMovies`, `SearchMovies`) and shared request helper methods.
