## ADDED Requirements

### Requirement: Reusable loading spinner component

The application SHALL provide a reusable `LoadingSpinner` component under `Components/UI` that renders an accessible, Bootstrap-styled spinner. The component MUST be usable in any Razor page or component without additional configuration.

#### Scenario: Default spinner renders with Bootstrap styling

- **WHEN** a consumer renders `<LoadingSpinner />` with no parameters set
- **THEN** the rendered markup contains a single root element with Bootstrap's `spinner-border` class
- **AND** the root element carries `role="status"`

#### Scenario: Spinner includes an accessible label

- **WHEN** a consumer renders `<LoadingSpinner />` with no parameters set
- **THEN** the rendered markup contains visually hidden text (Bootstrap's `visually-hidden` class) with the default label `"Loading..."`

### Requirement: Configurable label and size

The `LoadingSpinner` component SHALL expose public Razor parameters that allow consumers to configure the accessible label text and the visual size of the spinner. The component MUST use sensible defaults so that parameterless usage still renders correctly.

#### Scenario: Custom label is rendered in visually hidden text

- **WHEN** a consumer renders `<LoadingSpinner Label="Loading poster" />`
- **THEN** the visually hidden status text contains the string `"Loading poster"` instead of the default value

#### Scenario: Custom size is applied via inline CSS

- **WHEN** a consumer renders `<LoadingSpinner SizeRem="3" />`
- **THEN** the root spinner element's inline `style` attribute sets both `width` and `height` to `"3rem"`

#### Scenario: Default size is applied when SizeRem is not set

- **WHEN** a consumer renders `<LoadingSpinner />` with no `SizeRem` value
- **THEN** the root spinner element's inline `style` attribute sets both `width` and `height` to the component's documented default (`"2rem"`)

### Requirement: No external dependencies beyond existing Bootstrap

The `LoadingSpinner` component MUST rely only on the Bootstrap 5.3 CSS already loaded from `wwwroot/index.html` and on component-scoped CSS in `LoadingSpinner.razor.css`. Adding the component MUST NOT introduce new NuGet packages, new CDN references, or new global CSS files.

#### Scenario: Component avoids new runtime dependencies

- **WHEN** the `LoadingSpinner` component is added to the project
- **THEN** `NowPlayingApp.csproj` `PackageReference` entries are unchanged
- **AND** `wwwroot/index.html` is unchanged
- **AND** no new files are added under `wwwroot/css/` (aside from the component-scoped `LoadingSpinner.razor.css`, which is colocated with the component, not under `wwwroot/css/`)
