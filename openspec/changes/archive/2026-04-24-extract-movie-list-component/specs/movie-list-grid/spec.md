## ADDED Requirements

### Requirement: Responsive movie grid container

The `MovieList` component SHALL render a Bootstrap responsive grid container that hosts either a loading placeholder, a failure placeholder, or one `MovieCard` per `MovieResponse` in its `Movies.Results` collection. The grid MUST use the classes `row`, `g-3`, `row-cols-1`, `row-cols-sm-2`, `row-cols-lg-3`, and `row-cols-xl-4` so it matches the breakpoints in use on `Popular.razor` and `NowPlayingPage.razor` today.

#### Scenario: Populated response renders a card per movie

- **WHEN** a `MovieList` is rendered with `Movies` bound to a `MovieListResponse` whose `Results` contains `N` `MovieResponse` entries
- **THEN** the component renders the grid container `<div class="row g-3 row-cols-1 row-cols-sm-2 row-cols-lg-3 row-cols-xl-4">`
- **AND** the container contains exactly `N` direct child `<div class="col">` elements
- **AND** each `<div class="col">` contains a `MovieCard` bound to the corresponding `MovieResponse`

#### Scenario: Empty results collection renders a grid with no cards

- **WHEN** a `MovieList` is rendered with `Movies` bound to a `MovieListResponse` whose `Results` is an empty collection
- **THEN** the component renders the grid container with no child `<div class="col">` elements and no loading or failure placeholder

### Requirement: Loading placeholder while fetching

The `MovieList` component SHALL render a loading placeholder whenever `Movies` is `null` and `IsLoading` is `true`. The placeholder MUST appear inside the grid container and MUST contain an `<h2>` element whose text is `"Loading {CategoryLabel} movies..."` where `{CategoryLabel}` is the component's `CategoryLabel` parameter value.

#### Scenario: Loading placeholder is shown while fetch is in flight

- **WHEN** a `MovieList` is rendered with `Movies` set to `null`, `IsLoading` set to `true`, and `CategoryLabel` set to `"Popular"`
- **THEN** the component renders a placeholder wrapper `<div class="d-flex justify-content-center align-items-center">`
- **AND** the wrapper contains `<h2>Loading Popular movies...</h2>`
- **AND** no `MovieCard` elements are rendered

#### Scenario: Loading placeholder uses Now Playing label

- **WHEN** a `MovieList` is rendered with `Movies` set to `null`, `IsLoading` set to `true`, and `CategoryLabel` set to `"Now Playing"`
- **THEN** the component renders `<h2>Loading Now Playing movies...</h2>` inside the placeholder wrapper

### Requirement: Failure placeholder when fetch did not populate movies

The `MovieList` component SHALL render a failure placeholder whenever `Movies` is `null` and `IsLoading` is `false`. The placeholder MUST appear inside the grid container and MUST contain an `<h2 class="text-muted">` element whose text is `"Failed to load {CategoryLabel} movies."` where `{CategoryLabel}` is the component's `CategoryLabel` parameter value.

#### Scenario: Failure placeholder is shown when fetch completed without results

- **WHEN** a `MovieList` is rendered with `Movies` set to `null`, `IsLoading` set to `false`, and `CategoryLabel` set to `"Popular"`
- **THEN** the component renders a placeholder wrapper `<div class="d-flex justify-content-center align-items-center">`
- **AND** the wrapper contains `<h2 class="text-muted">Failed to load Popular movies.</h2>`
- **AND** no `MovieCard` elements are rendered

#### Scenario: Failure placeholder uses Now Playing label

- **WHEN** a `MovieList` is rendered with `Movies` set to `null`, `IsLoading` set to `false`, and `CategoryLabel` set to `"Now Playing"`
- **THEN** the component renders `<h2 class="text-muted">Failed to load Now Playing movies.</h2>` inside the placeholder wrapper

### Requirement: Public parameter contract

The `MovieList` component SHALL expose its inputs as three public Razor `[Parameter]` properties on the component class: `Movies` of type `MovieListResponse?`, `IsLoading` of type `bool`, and `CategoryLabel` of type `string`. The default values MUST be `null` for `Movies`, `false` for `IsLoading`, and the empty string `""` for `CategoryLabel` so the component can be constructed without arguments for testing.

#### Scenario: Default parameter values after construction

- **WHEN** a test instantiates `MovieList` directly without setting any parameters
- **THEN** `Movies` equals `null`
- **AND** `IsLoading` equals `false`
- **AND** `CategoryLabel` equals the empty string `""`

#### Scenario: Parameter round-trip through setters

- **WHEN** a test sets `Movies` to a non-null `MovieListResponse`, `IsLoading` to `true`, and `CategoryLabel` to `"Popular"` on a `MovieList` instance
- **THEN** reading those properties back returns the same instance, `true`, and `"Popular"` respectively

### Requirement: Page adoption on `Popular` and `NowPlayingPage`

The `Popular.razor` and `NowPlayingPage.razor` pages SHALL render the movie grid exclusively through the `MovieList` component. Neither page MAY retain an inline `@foreach` over `MovieListResponse.Results` or inline loading/failure `<h2>` placeholders that duplicate the responsibilities of `MovieList`.

#### Scenario: `Popular.razor` renders through `MovieList`

- **WHEN** a reader inspects the final `Popular.razor` markup
- **THEN** the page contains a `<MovieList ... />` element whose `Movies`, `IsLoading`, and `CategoryLabel` parameters are bound to the page's response field, loading flag, and the literal string `"Popular"`
- **AND** the page does not contain an inline `<div class="row g-3 row-cols-1 row-cols-sm-2 row-cols-lg-3 row-cols-xl-4">` or any `@foreach` iterating over `MovieListResponse.Results`

#### Scenario: `NowPlayingPage.razor` renders through `MovieList`

- **WHEN** a reader inspects the final `NowPlayingPage.razor` markup
- **THEN** the page contains a `<MovieList ... />` element whose `Movies`, `IsLoading`, and `CategoryLabel` parameters are bound to the page's response field, loading flag, and the literal string `"Now Playing"`
- **AND** the page does not contain an inline `<div class="row g-3 row-cols-1 row-cols-sm-2 row-cols-lg-3 row-cols-xl-4">` or any `@foreach` iterating over `MovieListResponse.Results`
