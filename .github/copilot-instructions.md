# Copilot Instructions (.NET)

These instructions guide GitHub Copilot when generating or modifying code in the
.NET backend of this repository. They describe general .NET conventions inferred from
the codebase and should be applied to any .NET work unless a more specific instruction
overrides them.

## Coding Style & Conventions

Follow the repository `.editorconfig`. Key points:

- **File-scoped namespaces** and 4-space indentation; `crlf` line endings.
- **Primary constructors** for dependency injection in endpoints and services
  (e.g., `public class GetDeviceByIdEndpoint(DataContext dataContext, IAuthService authService)`).
- Use `var` when the type is apparent; prefer language keywords (`string`, `int`)
  over BCL type names.
- Naming: `PascalCase` for types/methods/properties, `camelCase` for locals/parameters,
  `UPPER_CASE` for constants, `I`-prefixed interfaces, async methods suffixed `Async`.
- Prefer expression-bodied members, null-propagation, collection initializers, and
  pattern matching (`is null`, `is not null`) where they improve clarity.
- Use `nameof(...)` instead of hard-coded member/string names.
- Always honor `CancellationToken` parameters and pass them through async calls.
- Use XML doc comments (`/// <summary>`) on shared domain types and public contracts.
- Throw domain-specific exceptions from `Application/Exceptions` (e.g.,
  `EntityNotFoundException`, `InsufficientPermissionsException`) rather than generic ones.
- Keep `using` directives at the top of the file.

### Endpoint pattern (FastEndpoints)

Each endpoint is its own class that overrides `Configure()` (route, auth scheme,
group, OpenAPI summary/responses) and `HandleAsync(...)`. Route strings live in a
central `EndpointPaths` constants class. Validate permissions early, query via the
`DataContext`, map entities to DTOs, log the outcome, then send the response.

## Testing

- Place fast, isolated tests under `Unit/` and end-to-end/host tests under `Integration/`,
  mirroring the namespace of the code under test.
- Name tests `MethodName_Scenario_ExpectedResult` and structure bodies with
  `// Arrange`, `// Act`, `// Assert` comments.
- Use TUnit: `[Test]`, `[Category("...")]`, and `await Assert.That(actual).Is...()`.
- Cover the happy path plus edge cases (null/empty input, not-found, permission denied).
- Add or update tests alongside any behavioral change and ensure the suite builds and
  passes before completing work.

## Commit Messages

Generate commit messages following these rules:

1. **Format:** A concise summary line starting with a conventional commit prefix
   (e.g., `feat:`, `fix:`, `chore:`, `docs:`), followed by a blank line and a bulleted
   list of changes.
2. **Styling:** Use backticks (`` ` ``) for all file names, classes, methods, types,
   and variable names.
3. **List:** Use dashes (`-`) for the bulleted list of changes.
4. **Ordering:** Sort the bulleted list by importance, placing the most significant
   changes at the top and the least significant ones at the bottom.
5. **Language:** Use concise, imperative-style English.

### Example

```text
feat: add device history endpoint

- Add `GetDeviceHistoryEndpoint` exposing `GET /devices/{id}/history`
- Introduce `DeviceHistoryData` DTO in `MedHelp.Application`
- Register query filter handling in `DataContext`
- Add unit tests in `DeviceHistoryEntityTests`
```
