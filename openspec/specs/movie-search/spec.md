## ADDED Requirements

### Requirement: Top navigation search initiates movie query route

The top navigation component SHALL provide a movie-title search input and a search action that routes users to the search page using a query-string parameter. The route MUST use the format `/search?q=<query>` when the input is not empty.

#### Scenario: Search button navigates with query string

- **WHEN** a user enters a non-empty value in the top navigation search input and triggers the search action
- **THEN** the app navigates to `/search` with the `q` query-string parameter set to the entered value

#### Scenario: Empty search input does not navigate

- **WHEN** a user triggers the search action while the search input is empty
- **THEN** the app remains on the current route
- **AND** no `/search` navigation is initiated

### Requirement: Search page loads TMDB results from query parameter

The `/search` page SHALL read the `q` query-string value, call `TMDBClient.SearchMovies` with that value, and render the returned `MovieResponse` results through the shared `MovieList` component.

#### Scenario: Valid query renders matching movie cards

- **WHEN** `/search?q=<query>` is loaded with a non-empty `q` value and TMDB returns at least one matching result
- **THEN** `TMDBClient.SearchMovies` is called with the same query value
- **AND** the page renders `MovieList` using the returned results collection

#### Scenario: Valid query with no matches renders empty-state message

- **WHEN** `/search?q=<query>` is loaded and TMDB returns an empty results collection
- **THEN** the page renders a no-results message indicating no movies were found for the search

### Requirement: Search page handles invalid, cancelled, and failed requests safely

The `/search` page SHALL handle missing input and request failures without throwing unhandled exceptions and SHALL restore non-loading state after each request attempt.

#### Scenario: Missing or empty query short-circuits search request

- **WHEN** `/search` is loaded without a `q` value or with an empty `q` value
- **THEN** `TMDBClient.SearchMovies` is not called
- **AND** the page remains stable without a request attempt

#### Scenario: Cancelled search request is handled gracefully

- **WHEN** a search request is cancelled while the `/search` page is active
- **THEN** the page catches the cancellation and does not crash
- **AND** the page exits loading state

#### Scenario: HTTP failure during search is handled gracefully

- **WHEN** `TMDBClient.SearchMovies` throws `HttpRequestException`
- **THEN** the page catches and logs the error
- **AND** the page exits loading state without an unhandled exception

### Requirement: TMDB client supports search movie endpoint

`TMDBClient` SHALL expose a `SearchMovies` operation that validates input and requests TMDB's `search/movie` endpoint with standard query parameters used by the app (`include_adult=false`, `language=en-US`, and `page=1`).

#### Scenario: Search request uses expected endpoint and query parameters

- **WHEN** `TMDBClient.SearchMovies("batman")` is called
- **THEN** the underlying HTTP request targets `search/movie` with query `query=batman`
- **AND** the request includes `include_adult=false`, `language=en-US`, and `page=1`

#### Scenario: Empty query input is rejected

- **WHEN** `TMDBClient.SearchMovies` is called with an empty query string
- **THEN** the method throws `ArgumentException` before performing an HTTP request

#### Scenario: Null-deserialized response throws request exception

- **WHEN** TMDB responds successfully but the response body deserializes to `null`
- **THEN** `TMDBClient.SearchMovies` throws `HttpRequestException`
