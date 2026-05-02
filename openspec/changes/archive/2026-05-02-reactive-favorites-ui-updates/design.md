## Context

Favorites in `NowPlayingApp` are persisted through `FavoritesService` and rendered from multiple component trees:

- `MovieCard` determines button text from a local `IsFavorite` field.
- `Favorites.razor` loads `_favoriteMovies` from `IFavoritesService.GetFavoritesAsync()` during `OnParametersSetAsync`.
- `NowPlayingPage.razor` and `Popular.razor` render `MovieCard` through `MovieList`.

The current implementation writes changes to local storage but does not broadcast state changes to components. As a result:

- `MovieCard` button text does not update after click until lifecycle re-entry (for example, refresh).
- `Favorites.razor` does not remove cards immediately after a movie is unfavorited.

This change introduces reactive favorites notifications so all affected components update in place.

## Goals / Non-goals

Goals:

- Make favorite toggles immediately update `MovieCard` button labels.
- Make `/favorites` immediately remove an unfavorited movie from the rendered list.
- Keep favorites behavior consistent across `NowPlayingPage`, `Popular`, and `Favorites`.
- Keep persistence model unchanged (local storage through `LocalStorageService`).
- Add focused tests that prevent regressions in reactive behavior.

Non-goals:

- Reworking page fetch lifecycles unrelated to favorites.
- Replacing local storage with server persistence.
- Introducing authentication or user profile favorites sync.
- Redesigning button styling or card layout.
- Changes to sibling projects outside `NowPlayingApp` and `NowPlayingApp.Tests`.

## Decisions

### Decision 1: Add service-level favorites change notifications

`IFavoritesService` will expose a notification mechanism that components can subscribe to when favorites are mutated. `FavoritesService` will raise this notification after successful add/remove/save operations.

Rationale:

- The favorite state is shared across independent pages/components.
- Service-level notifications avoid brittle parent-child callback chains across route boundaries.
- This model keeps persistence and state signaling co-located.

Alternatives considered:

- Parent callback from `MovieCard` to page (`EventCallback` only). Rejected because it does not naturally synchronize with other pages/components.
- Polling local storage on a timer. Rejected for unnecessary complexity and stale UI windows.

### Decision 2: Keep `MovieCard` as source of button text, but refresh local state after toggles

`MovieCard` retains its local `IsFavorite` property for rendering `Add favorite`/`Remove favorite`. After add/remove actions it will re-evaluate favorite status and request re-render. It will also subscribe to service notifications so cross-component changes are reflected.

Rationale:

- Minimal change to existing component contract and markup.
- Immediate local update removes click-to-refresh mismatch.
- Subscription keeps text correct when another component changes favorites.

Alternatives considered:

- Drive `IsFavorite` as required parent parameter. Rejected because pages currently do not track per-card favorite state.

### Decision 3: `Favorites.razor` subscribes to favorites changes and refreshes its list

`Favorites.razor` will subscribe to favorites notifications and refresh `_favoriteMovies` whenever favorites mutate. It will dispose subscription on component disposal.

Rationale:

- Ensures removed favorites disappear immediately on `/favorites`.
- Keeps page behavior simple and reliable even if mutations occur outside the page.

Alternatives considered:

- Local list mutation only (remove the clicked movie from `_favoriteMovies`). Rejected as sole strategy because it misses external changes.

### Decision 4: Preserve DI and configuration footprint

No changes to `Program.cs` registrations, `TMDBClient`, or configuration are needed. `IFavoritesService` remains scoped and local-storage-backed.

Rationale:

- The issue is reactive UI state propagation, not service registration or API configuration.
- Limits blast radius and implementation risk.

## Risks / Trade-offs

- Risk: Event subscriptions can leak if components do not unsubscribe. Mitigation: implement `IDisposable` on subscribing components and detach handlers.
- Risk: Multiple rapid clicks could trigger overlapping async operations and transient labels. Mitigation: update state after operation completion and rely on service notification to converge state.
- Trade-off: Event-based state sync introduces some lifecycle complexity. Benefit outweighs complexity for cross-page consistency.

## Migration Plan

This is a UI behavior change only. No schema, API, or config migration is required.

Rollout:

- Implement service notification surface and component subscriptions.
- Run unit tests and manual UI validation across `/favorites`, `/now-playing`, and `/popular`.

Rollback:

- Revert the single change commit; no persisted-data migration rollback is required.

## Open Questions

- None.

## Resolved Decisions

- Keep the current favorite button text and casing as-is in this change.
- Defer disabling favorite button interactions during in-flight add/remove operations.
