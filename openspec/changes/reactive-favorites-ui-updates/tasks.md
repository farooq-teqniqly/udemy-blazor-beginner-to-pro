## 1. Add reactive notification contract to favorites service

- [x] 1.1 Update `UdemyBlazorBeginnerToPro/NowPlayingApp/Services/IFavoritesService.cs` to expose a favorites-changed notification surface that components can subscribe to.
- [x] 1.2 Update `UdemyBlazorBeginnerToPro/NowPlayingApp/Services/FavoritesService.cs` to raise favorites-changed notifications after successful add/remove/save operations.
- [x] 1.3 Ensure add/remove logic consistently compares movies by `Id` (not reference equality) so duplicate and stale-entry behavior is deterministic.
- [x] 1.4 Preserve existing local-storage error handling and logging while adding notifications.

## 2. Make `MovieCard` update button text immediately

- [x] 2.1 In `UdemyBlazorBeginnerToPro/NowPlayingApp/Components/UI/MovieCard.razor.cs`, update `HandleToggleFavorite` so `IsFavorite` is refreshed immediately after add/remove completes.
- [x] 2.2 Subscribe `MovieCard` to favorites-changed notifications so button text stays correct when favorites are changed elsewhere.
- [x] 2.3 Add disposal logic in `MovieCard` to unsubscribe from favorites notifications and avoid leaks.
- [x] 2.4 Keep button text literals and markup in `MovieCard.razor` unchanged unless required for behavior correctness.

## 3. Make Favorites page remove cards without refresh

- [x] 3.1 In `UdemyBlazorBeginnerToPro/NowPlayingApp/Components/Pages/Favorites.razor`, subscribe to favorites-changed notifications.
- [x] 3.2 Implement refresh logic that updates `_favoriteMovies` and triggers re-render when notifications are received.
- [x] 3.3 Ensure `Favorites.razor` unsubscribes on disposal.
- [x] 3.4 Keep existing loading and error logging behavior intact while adding reactive updates.

## 4. Add tests for reactive favorites behavior

- [x] 4.1 Add or update tests in `UdemyBlazorBeginnerToPro/NowPlayingApp.Tests` for favorites service notification behavior after add/remove/save.
- [x] 4.2 Add or update `MovieCard` tests to verify favorite-toggle paths update `IsFavorite` and button state inputs without requiring a component refresh cycle.
- [x] 4.3 Add or update `Favorites` page/component tests to verify removed favorites no longer appear after service notifications.

## 5. Verification

- [x] 5.1 Run `dotnet build UdemyBlazorBeginnerToPro/UdemyBlazorBeginnerToPro.slnx` and confirm the solution builds cleanly.
- [x] 5.2 Run `dotnet test UdemyBlazorBeginnerToPro/NowPlayingApp.Tests/NowPlayingApp.Tests.csproj` and confirm updated tests pass.
- [x] 5.3 Run the app and validate behavior manually:
  - Remove favorite on `/favorites` and confirm card disappears immediately.
  - Add favorite on `/now-playing` and `/popular` and confirm button text changes immediately.
  - Remove favorite on `/now-playing` and `/popular` and confirm button text changes back immediately.
- [x] 5.4 Confirm no new `PackageReference` entries were added to app or test csproj files.
- [x] 5.5 Confirm no changes were made under `UdemyBlazorBeginnerToPro/NowPlayingApp/BrunoTests/TMDB/`.

## 6. Scope constraints (resolved decisions)

- [x] 6.1 Keep favorite button text and casing unchanged in `MovieCard.razor` (`Add favorite` / `Remove favorite`).
- [x] 6.2 Do not add in-flight disable behavior for favorite buttons in this change; defer it to a follow-up if needed.
