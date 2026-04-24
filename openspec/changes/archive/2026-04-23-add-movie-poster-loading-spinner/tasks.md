## 1. Reusable `LoadingSpinner` component

- [x] 1.1 Create `UdemyBlazorBeginnerToPro/NowPlayingApp/Components/UI/LoadingSpinner.razor` with two `[Parameter]` properties - `Label` (default `"Loading..."`) and `SizeRem` (default `2`) - rendering Bootstrap's `spinner-border` with `role="status"`, a visually hidden label, and an inline `style` setting `width`/`height` from `SizeRem`.
- [x] 1.2 Create `UdemyBlazorBeginnerToPro/NowPlayingApp/Components/UI/LoadingSpinner.razor.css` with any component-scoped styles needed for centering and to harmonize the spinner color with the app's primary color palette.
- [x] 1.3 Confirm `UdemyBlazorBeginnerToPro/NowPlayingApp/_Imports.razor` already exposes `NowPlayingApp.Components.UI`; leave it unchanged if present or add the namespace if the component moves outside that folder.

## 2. `MovieCard` poster loading state

- [x] 2.1 Update `UdemyBlazorBeginnerToPro/NowPlayingApp/Components/UI/MovieCard.razor` to wrap the existing `<img>` in a `movie-card-poster` container that reserves the TMDB portrait (`2 / 3`) aspect ratio and absolutely positions the `<img>`.
- [x] 2.2 Add a private `bool _isPosterLoading = true` field to `MovieCard` and wire `@onload` and `@onerror` on the `<img>` to handlers that set `_isPosterLoading = false`.
- [x] 2.3 Render `<LoadingSpinner />` overlay inside the poster container while `_isPosterLoading` is `true`, and set `aria-busy="true"` on the container for that state.
- [x] 2.4 Ensure the `<img>` is visually hidden (for example, `visibility: hidden`) while `_isPosterLoading` is `true` so the spinner is the only visible element in the poster area.
- [x] 2.5 Update `UdemyBlazorBeginnerToPro/NowPlayingApp/Components/UI/MovieCard.razor.css` with the new `.movie-card-poster` container rules (relative positioning, `aspect-ratio: 2 / 3`, centered spinner), keeping existing card hover/transform styles intact.
- [x] 2.6 Set the `<img>`'s `alt` attribute to `@Movie.Title` so screen readers announce the movie title.
- [x] 2.7 Handle the `@onerror` path so that when a remote poster fails to load, `_isPosterLoading` becomes `false` and the `<img>` `src` falls back to `/images/poster.png` (for example, by swapping `src` via a private field bound to the `<img>`).

## 3. Tests for `LoadingSpinner`

- [x] 3.1 Add `UdemyBlazorBeginnerToPro/NowPlayingApp.Tests/Components/UI/LoadingSpinnerTests.cs` with xUnit `[Fact]` tests that instantiate `LoadingSpinner` directly and assert `Label` defaults to `"Loading..."` and `SizeRem` defaults to `2`.
- [x] 3.2 Add an xUnit `[Fact]` that sets `Label = "Loading poster"` and `SizeRem = 3` on a `LoadingSpinner` instance and asserts the getters return those values, documenting the public parameter contract.

## 4. Tests for `MovieCard` poster state

- [x] 4.1 If not already present, expose a testable seam on `MovieCard` for the load state transition (either an `internal` setter behind `InternalsVisibleTo("NowPlayingApp.Tests")` or a small `internal void HandlePosterLoaded()` helper). Avoid making production code overly public.
- [x] 4.2 Add `UdemyBlazorBeginnerToPro/NowPlayingApp.Tests/Components/UI/MovieCardTests.cs` with an xUnit `[Fact]` that constructs a `MovieCard` with a `TMDBClient` whose `IOptions<TMDBClientSettings>` is mocked with NSubstitute, and asserts the initial `_isPosterLoading` state is `true` (named `IsPosterLoading_When_CardRendered_Returns_True`).
- [x] 4.3 Add an xUnit `[Fact]` named `IsPosterLoading_When_PosterLoadEventHandled_Returns_False` that invokes the load-handler seam and asserts the state transitions to `false`.
- [x] 4.4 Add an xUnit `[Fact]` named `IsPosterLoading_When_PosterErrorEventHandled_Returns_False` that invokes the error-handler seam and asserts the state transitions to `false` and that the effective `<img>` source is `/images/poster.png` (via whatever internal property exposes it).

## 5. Verification

- [x] 5.1 Run `dotnet build UdemyBlazorBeginnerToPro/UdemyBlazorBeginnerToPro.slnx` and confirm the solution builds with zero warnings introduced by this change.
- [x] 5.2 Run `dotnet test UdemyBlazorBeginnerToPro/NowPlayingApp.Tests/NowPlayingApp.Tests.csproj` and confirm all new tests pass together with the existing test suite.
- [ ] 5.3 Run `UdemyBlazorBeginnerToPro/NowPlayingApp` locally, navigate to `/now-playing`, enable the browser dev tools' "Slow 3G" network throttling, and confirm each `MovieCard` shows a spinner until its poster loads, with no layout shift and no spinner left running on cards whose images loaded or errored.
- [x] 5.4 Confirm that no new NuGet `PackageReference` entries were added to `UdemyBlazorBeginnerToPro/NowPlayingApp/NowPlayingApp.csproj` or `UdemyBlazorBeginnerToPro/NowPlayingApp.Tests/NowPlayingApp.Tests.csproj`, matching the "no new dependencies" decision in `design.md`.
- [x] 5.5 Confirm there are no changes to `UdemyBlazorBeginnerToPro/NowPlayingApp/BrunoTests/TMDB/` because this change does not consume any new TMDB endpoints.
