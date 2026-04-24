## ADDED Requirements

### Requirement: Poster loading indicator

The `MovieCard` component SHALL display a visible loading indicator inside the poster area from the moment the card renders until the underlying poster `<img>` element has finished loading or has errored. While the indicator is visible, the poster area MUST reserve the same space it will occupy once the image has loaded so that surrounding card content does not shift.

#### Scenario: Spinner is shown while poster is loading

- **WHEN** a `MovieCard` first renders for a `MovieResponse` whose `PosterPath` resolves to a remote TMDB image URL
- **THEN** the poster area renders a `LoadingSpinner` overlay and the `<img>` element is either hidden from view or has zero visual opacity
- **AND** the card's overall width and height are the same as when the image has finished loading

#### Scenario: Spinner is hidden after poster loads

- **WHEN** the underlying poster `<img>` element's `load` event has fired
- **THEN** the `LoadingSpinner` overlay is removed from the DOM
- **AND** the loaded poster image is visible inside the card

#### Scenario: Spinner is hidden after poster errors

- **WHEN** the underlying poster `<img>` element's `error` event has fired
- **THEN** the `LoadingSpinner` overlay is removed from the DOM
- **AND** the card displays the local fallback image `wwwroot/images/poster.png` in place of the failed remote poster

### Requirement: Fallback image when poster path is missing

The `MovieCard` component SHALL display the local fallback image `wwwroot/images/poster.png` whenever `TMDBClient.GetPosterUri` resolves to that relative URI (that is, when `MovieResponse.PosterPath` is null, empty, or cannot produce a remote URL). The loading indicator MUST NOT remain visible indefinitely for the fallback image.

#### Scenario: Missing poster path resolves to the fallback image

- **WHEN** a `MovieCard` renders for a `MovieResponse` whose `PosterPath` is null or the empty string
- **THEN** the `<img>` element's `src` attribute equals the URI returned by `TMDBClient.GetPosterUri("")`, which is the relative URI `/images/poster.png`
- **AND** once that local image has loaded, the `LoadingSpinner` overlay is no longer displayed

### Requirement: Accessible poster image

The `MovieCard` component SHALL render the poster `<img>` with an `alt` attribute that conveys the movie title to assistive technologies, and the loading indicator MUST be announced as a busy state while it is visible.

#### Scenario: Poster has a descriptive alt text

- **WHEN** a `MovieCard` renders for a `MovieResponse` whose `Title` is `"Example Movie"`
- **THEN** the poster `<img>` element's `alt` attribute contains the text `"Example Movie"`

#### Scenario: Loading state is announced to assistive technology

- **WHEN** a `MovieCard` is rendering with the loading indicator visible
- **THEN** the poster area exposes `aria-busy="true"` and the inner `LoadingSpinner` is rendered with `role="status"`
