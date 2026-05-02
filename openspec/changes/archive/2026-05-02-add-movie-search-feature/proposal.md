## Why

Users can browse Now Playing and Popular movies, but they cannot directly find a specific title unless it appears in those lists. This change adds a search flow that lets users query TMDB by title and view matching results inside the app.

## What Changes

- Add a search entry point in the top navigation so users can submit a movie-title query.
- Add a dedicated `/search` page that reads the `q` query-string value, requests TMDB search results, and renders them using the shared movie-list UI.
- Add `TMDBClient.SearchMovies` support for TMDB's movie-search endpoint.
- Add request-state handling on the search page for empty query values, loading completion, cancelled requests, and TMDB HTTP errors.
- Add test coverage in `NowPlayingApp.Tests` for TMDB search requests and search-page component behavior.

## Capabilities

### New Capabilities

- `movie-search`: Users can search for movies by title from the app navigation and view TMDB-backed results on the search page.

### Modified Capabilities

- None. Existing capabilities are not changed at the requirement level.

## Impact

- Affected code:
  - `UdemyBlazorBeginnerToPro/NowPlayingApp/Components/Layout/TopNavMenu.razor`
  - `UdemyBlazorBeginnerToPro/NowPlayingApp/Components/Pages/Search.razor`
  - `UdemyBlazorBeginnerToPro/NowPlayingApp/Components/Pages/Search.razor.cs`
  - `UdemyBlazorBeginnerToPro/NowPlayingApp/Services/TMDBClient.cs`
  - `UdemyBlazorBeginnerToPro/NowPlayingApp.Tests/TMDBClientTests.cs`
  - `UdemyBlazorBeginnerToPro/NowPlayingApp.Tests/Components/Pages/SearchTests.cs`
- Affected APIs: TMDB API `search/movie` endpoint is now used by the client.
- Dependencies: No new NuGet packages required.
- Configuration and secrets: No new configuration keys or user-secrets values are required. Existing TMDB access token configuration continues to be used.

## Non-goals

- Adding advanced TMDB search filters (year, genre, language, region, adult-content toggle, pagination controls).
- Persisting or sharing search history across sessions.
- Redesigning movie cards or the broader application layout.
- Any changes to sibling projects outside `NowPlayingApp` and `NowPlayingApp.Tests`, including:
  - `UdemyBlazorBeginnerToPro/BlazorLayout`
  - `UdemyBlazorBeginnerToPro/Fizzbuzz` and `UdemyBlazorBeginnerToPro/Fizzbuzz.Tests`
  - `UdemyBlazorBeginnerToPro/LoanShark` and `UdemyBlazorBeginnerToPro/LoanShark.Tests`
  - `UdemyBlazorBeginnerToPro/Tasker`
  - `UdemyBlazorBeginnerToPro/templates`
