## Why

Users can add and remove favorites, but the UI does not react immediately to those actions. On `/favorites`, removing a movie keeps the card visible until a browser refresh. On `/now-playing` and `/popular`, clicking the favorite button does not update the label from "Add favorite" to "Remove favorite" (or back) until refresh. This creates the impression that the action failed, even though local storage was updated.

## What Changes

- Introduce reactive favorites state notifications in the favorites service so interested components can update when favorites change.
- Update `MovieCard` to refresh its local `IsFavorite` state after add/remove actions so button text updates immediately.
- Update `Favorites.razor` to react to favorites changes and refresh or locally update its list without requiring page refresh.
- Ensure pages that render `MovieCard` (`NowPlayingPage.razor`, `Popular.razor`, and `Favorites.razor` via `MovieList`) stay synchronized after toggling favorites in any location.
- Add test coverage in `NowPlayingApp.Tests` for immediate button-label updates and favorites page list updates after favorite removal.

## Capabilities

### New Capabilities

- `favorites-ui-reactivity`: Favorite toggles immediately update UI state across pages, including button labels and the Favorites list, without navigation or browser refresh.

### Modified Capabilities

- None. Existing capabilities are extended by behavior improvements only.

## Impact

- Affected code:
  - `UdemyBlazorBeginnerToPro/NowPlayingApp/Services/IFavoritesService.cs`
  - `UdemyBlazorBeginnerToPro/NowPlayingApp/Services/FavoritesService.cs`
  - `UdemyBlazorBeginnerToPro/NowPlayingApp/Components/UI/MovieCard.razor.cs`
  - `UdemyBlazorBeginnerToPro/NowPlayingApp/Components/Pages/Favorites.razor`
  - `UdemyBlazorBeginnerToPro/NowPlayingApp.Tests` favorites-related component/service tests
- Affected APIs: Internal app service contract (`IFavoritesService`) and component interaction behavior.
- Dependencies: No new NuGet packages expected.
- Configuration and secrets: No TMDB endpoint changes, no new appsettings keys, and no new user-secrets values.

## Non-goals

- Redesigning the visual style, text casing, or layout of favorite buttons and movie cards.
- Refactoring all page data-loading patterns beyond favorites reactivity needs.
- Adding server-side persistence for favorites or syncing favorites between devices.
- Introducing authentication/authorization for favorites.
- Any changes to sibling projects outside `NowPlayingApp` and `NowPlayingApp.Tests`, including:
  - `UdemyBlazorBeginnerToPro/BlazorLayout`
  - `UdemyBlazorBeginnerToPro/Fizzbuzz` and `UdemyBlazorBeginnerToPro/Fizzbuzz.Tests`
  - `UdemyBlazorBeginnerToPro/LoanShark` and `UdemyBlazorBeginnerToPro/LoanShark.Tests`
  - `UdemyBlazorBeginnerToPro/Tasker`
  - `UdemyBlazorBeginnerToPro/templates`
