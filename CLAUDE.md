# Claude Code Guidelines

## Document Formatting

- No emojis. Use regular bullets.
- No em dashes. Use regular hyphens.
- US English spelling and grammar.

## Documentation

- All public and internal classes, methods, and properties must have XML doc comments (`/// <summary>`).
- Use complete sentences ending with a period.
- Do not restate the identifier name verbatim - describe behavior, purpose, or contract.
- Document parameters (`<param>`), return values (`<returns>`), and exceptions (`<exception>`) when they add information beyond what the signature already conveys.

## Testing

- Prefer data-driven tests over individual tests when testing multiple inputs or variations of the same behavior. Use `[Theory]` with `[InlineData]`, `[MemberData]`, or `[ClassData]` (xUnit) rather than multiple `[Fact]` methods that differ only in their inputs.
- All test methods must have Arrange/Act/Assert comments. Use `// Arrange`, `// Act`, `// Assert` as separate sections. Use `// Act & Assert` when the act and assert cannot be separated (e.g., `Assert.Throws`, single-line `[Theory]` bodies, no-throw checks). Omit `// Arrange` only when there is no local setup (e.g., shared static test data at class level).

## Naming Conventions

Sources: [.NET Framework Design Guidelines](https://learn.microsoft.com/en-us/dotnet/standard/design-guidelines/naming-guidelines) and [ASP.NET Core Engineering Guidelines](https://github.com/dotnet/aspnetcore/wiki/Engineering-guidelines#coding-guidelines). Where these conflict, ASP.NET Core guidelines take precedence.

### Capitalization by identifier kind

| Identifier | Casing | Example |
| --- | --- | --- |
| Namespace | PascalCase | `NowPlayingApp.Services` |
| Class / struct | PascalCase | `TMDBClient` |
| Interface | PascalCase with `I` prefix | `IMovieService` |
| Method | PascalCase | `GetNowPlayingAsync` |
| Property | PascalCase | `AccessToken` |
| Event | PascalCase | `MovieLoaded` |
| Public / static / const field | PascalCase | `InfiniteTimeout` |
| Private instance field | `_camelCase` | `_httpClient` |
| Parameter | camelCase | `cancellationToken` |
| Local variable | camelCase (use `var`) | `var result = ...` |
| Enum value | PascalCase | `FileMode.Append` |

### Capitalization rules

- Two-letter acronyms: capitalize both letters (`IOStream`, `UI`). Longer acronyms: capitalize only the first letter (`HtmlTag`, `XmlReader`).
- Closed-form compound words are treated as a single word. Examples: `Endpoint` not `EndPoint`, `Callback` not `CallBack`, `FileName` not `Filename`, `Metadata` not `MetaData`, `UserName` not `Username`, `SignIn` not `SignOn`.
- Names must not differ by case alone.

### General rules

- Use `var` as much as the compiler will allow (ASP.NET Core). Exceptions: null assignments without a cast, `const` declarations.
- Use C# type keywords, not .NET type names: `string` not `String`, `int` not `Int32`, `bool` not `Boolean`.
- Favor readability over brevity: `CanScrollHorizontally` is better than `ScrollableX`.
- Use only complete words or widely accepted standard abbreviations in public APIs: `AddReference()` not `AddRef()`, `SomeObject` not `SomeObj`.
- No Hungarian notation.
- No abbreviations or contractions in identifiers unless the abbreviation is a well-known industry standard.
- No underscores, hyphens, or other non-alphanumeric characters in public identifiers. Exception: `_camelCase` for private instance fields (ASP.NET Core).
- Always specify member visibility explicitly, even when it is the default: `private string _foo;` not `string _foo;` (ASP.NET Core).
- Avoid `this.` unless required to disambiguate.

### Namespaces

- Format: `<Company>.<Product>[.<Feature>][.<Subnamespace>]`
- Use PascalCase with period separators.
- Do not name a namespace the same as a type it contains.
- Do not use generic type names as top-level type names (`Element`, `Node`, `Log`, `Message`) - qualify them.

### Classes and structs

- Use nouns or noun phrases.
- No `C` prefix or any other type prefix.
- Consider ending a derived class name with the base class name: `ArgumentOutOfRangeException`, `SerializableAttribute`.

### Interfaces

- Prefix with `I`: `IMovieService`, `IEnumerable`.
- Use adjective phrases or noun phrases.
- When defining a class-interface pair where the class is the standard implementation, the names differ only by the `I` prefix: `IFoo` / `Foo`.

### Methods

- Use verbs or verb phrases: `GetNowPlaying`, `LoadMovieDetails`.
- All async methods must have the `Async` suffix (ASP.NET Core): `GetNowPlayingAsync`.
- Async methods accept an optional `CancellationToken` parameter with a default value of `CancellationToken.None`.

### Properties

- Use nouns, noun phrases, or adjectives.
- Boolean properties use an affirmative phrase: `CanSeek` not `CantSeek`. Optionally prefix with `Is`, `Can`, or `Has` where it adds value.
- Collection properties use a plural phrase: `Movies` not `MovieList` or `MovieCollection`.
- Do not create a property with the same name as a corresponding `Get` method on the same type.

### Events

- Use verbs or verb phrases: `Clicked`, `Painting`, `DroppedDown`.
- Use present and past tense for before/after pairs: `Closing` / `Closed`. Do not use `Before`/`After` prefixes.
- Event handler delegates use the `EventHandler` suffix: `ClickedEventHandler`.
- Event argument classes use the `EventArgs` suffix: `ClickedEventArgs`.
- Event handler parameters are named `sender` and `e`.

### Fields

- Public and protected static fields: PascalCase.
- Private instance fields: `_camelCase` (ASP.NET Core).
- No `g_`, `s_`, or other prefixes on static fields.
- No public or protected instance fields on classes (use properties).

### Parameters

- camelCase.
- Use descriptive names based on meaning, not type: `movies` not `list`, `cancellationToken` not `ct`.
- No abbreviations or numeric indices.

### Generic type parameters

- Use `T` for a single-letter type parameter: `IComparer<T>`.
- Prefix descriptive type parameter names with `T`: `ISessionChannel<TSession>`.
- Consider encoding constraints in the name: a parameter constrained to `ISession` is `TSession`.

### Type suffixes and naming by base type

| Base type | Rule |
| --- | --- |
| `System.Attribute` | Suffix `Attribute` |
| Event delegate | Suffix `EventHandler` |
| Non-event delegate | Suffix `Callback` |
| `System.EventArgs` | Suffix `EventArgs` |
| `System.Exception` | Suffix `Exception` |
| `IDictionary` / `IDictionary<K,V>` | Suffix `Dictionary` |
| `IEnumerable` / `ICollection` / `IList` variants | Suffix `Collection` |
| `System.IO.Stream` | Suffix `Stream` |

### Enumerations

- Singular name for a regular enum: `FileMode`.
- Plural name for a flags enum: `FileAttributes`.
- No `Enum`, `Flag`, or `Flags` suffix.
- No prefix on enum values.

### Extension methods

- Use only when a regular static method would not suffice (ASP.NET Core).
- Naming pattern: `<Feature>Extensions`, `<Target><Feature>Extensions`, or `<Feature><Target>Extensions`.
- Namespace matches the feature, not the target type.
