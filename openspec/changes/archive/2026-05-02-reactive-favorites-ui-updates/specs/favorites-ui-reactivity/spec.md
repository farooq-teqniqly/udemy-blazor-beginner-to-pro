## ADDED Requirements

### Requirement: Favorite toggle updates button text immediately

When a user toggles favorite state from a `MovieCard`, the card SHALL update its favorite action button text in the same interaction cycle, without requiring browser refresh or page navigation.

#### Scenario: Add favorite updates button text to remove action

- **GIVEN** a `MovieCard` renders a movie that is not currently in favorites
- **WHEN** the user clicks the favorite action button
- **THEN** the movie is persisted to favorites
- **AND** the same `MovieCard` updates its button text from `"Add favorite"` to `"Remove favorite"` without refresh

#### Scenario: Remove favorite updates button text to add action

- **GIVEN** a `MovieCard` renders a movie that is currently in favorites
- **WHEN** the user clicks the favorite action button
- **THEN** the movie is removed from favorites
- **AND** the same `MovieCard` updates its button text from `"Remove favorite"` to `"Add favorite"` without refresh

### Requirement: Favorites page reflects removals immediately

The `/favorites` page SHALL remove a movie card from the rendered favorites list immediately after that movie is unfavorited, without requiring browser refresh.

#### Scenario: Remove favorite from favorites page removes card immediately

- **GIVEN** `/favorites` is showing a movie in the list
- **WHEN** the user clicks `"Remove favorite"` for that movie
- **THEN** the movie is removed from favorites persistence
- **AND** the movie card no longer appears in the favorites list without refresh

### Requirement: Favorites state changes propagate across pages

Components that depend on favorite state SHALL react to favorites mutations from the shared favorites service so UI state remains consistent across `Popular`, `Now Playing`, and `Favorites`.

#### Scenario: Add favorite on now playing is reflected by card state

- **GIVEN** the user is on `/now-playing` and a movie is not a favorite
- **WHEN** the user clicks `"Add favorite"`
- **THEN** the clicked card immediately shows `"Remove favorite"`
- **AND** if that movie is rendered again by a subscribed favorites-dependent component, it resolves as favorite without refresh

#### Scenario: Remove favorite on popular is reflected by card state

- **GIVEN** the user is on `/popular` and a movie is currently a favorite
- **WHEN** the user clicks `"Remove favorite"`
- **THEN** the clicked card immediately shows `"Add favorite"`
- **AND** subscribed favorites-dependent components resolve that movie as not favorite without refresh

### Requirement: Favorites service exposes mutation notifications

The favorites service contract SHALL provide a notification mechanism for favorite mutations so components can subscribe and react to state changes.

#### Scenario: Add favorite emits favorites-changed notification

- **WHEN** `IFavoritesService.AddFavoriteAsync` successfully adds a movie
- **THEN** the service emits a favorites-changed notification

#### Scenario: Remove favorite emits favorites-changed notification

- **WHEN** `IFavoritesService.RemoveFavoriteAsync` successfully removes a movie
- **THEN** the service emits a favorites-changed notification

#### Scenario: Subscribing component can unsubscribe safely

- **GIVEN** a component subscribed to favorites-changed notifications
- **WHEN** the component is disposed
- **THEN** it can unsubscribe without exceptions
- **AND** subsequent favorites changes do not invoke that component's handler
