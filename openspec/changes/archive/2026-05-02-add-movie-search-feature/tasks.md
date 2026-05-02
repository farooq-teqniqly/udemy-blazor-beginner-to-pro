## 1. Search Navigation and Page UX

- [x] 1.1 Add search input handling in `Components/Layout/TopNavMenu.razor` so non-empty input navigates to `/search?q=<query>` and empty input does not navigate.
- [x] 1.2 Create `Components/Pages/Search.razor` to render search-page headings, empty-state messaging, and shared `MovieList` result rendering.
- [x] 1.3 Implement search-page lifecycle logic in `Components/Pages/Search.razor.cs` for query binding, loading state, cancellation, and HTTP error handling.

## 2. TMDB Search API Integration

- [x] 2.1 Add `SearchMovies(string query, CancellationToken cancellationToken = default)` to `Services/TMDBClient.cs` targeting TMDB `search/movie` with `include_adult=false`, `language=en-US`, and `page=1`.
- [x] 2.2 Keep existing DI registration in `Program.cs` and confirm search traffic uses the existing typed `TMDBClient` registration with `AddStandardResilienceHandler`.
- [x] 2.3 Add or update a Bruno request under `NowPlayingApp/BrunoTests/TMDB` for TMDB `search/movie` endpoint usage.

## 3. Automated Test Coverage

- [x] 3.1 Add `SearchMovies` unit tests in `NowPlayingApp.Tests/TMDBClientTests.cs` for successful responses, empty query guard clauses, and null-deserialized response failures.
- [x] 3.2 Add component tests in `NowPlayingApp.Tests/Components/Pages/SearchTests.cs` for empty query behavior, successful result loading, cancellation handling, HTTP failure handling, and dispose safety.
- [x] 3.3 Run `dotnet test` for `NowPlayingApp.Tests` and confirm all tests pass after search feature additions.
