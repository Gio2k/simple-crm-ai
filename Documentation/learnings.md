# Implementation Learnings

This document captures lessons learned during feature slice implementation.

---

## CreateLead Slice (Phase 1)

### LanguageExt 4.x API

- **Tuple-based `Apply`** works for accumulating validation errors across multiple fields:
  ```csharp
  (ValidateName(request.Name), ValidateEmail(request.Email))
      .Apply((_, _) => request)
  ```
  Use `_` discards when the validated values aren't needed — the original `request` is returned unchanged.

- **`Fail` and `Success` helpers** require `using static LanguageExt.Prelude;`:
  ```csharp
  Fail<Error, string>(Error.New("Name is required"))
  Success<Error, string>(name)
  ```

- **`Validation<Error, T>.Match`** receives `Seq<Error>` (not `IEnumerable`) in the `Fail` branch:
  ```csharp
  result.Match(
      Succ: _ => ...,
      Fail: errors => ...   // errors is Seq<Error>
  )
  ```

- **Async `Match` on `Validation`** works by making both branches return `Task<T>`:
  ```csharp
  await validation.Match(
      Succ: async req => { ... return Results.Created(...); },
      Fail: errors => Task.FromResult<IResult>(Results.ValidationProblem(...))
  );
  ```

### Email Validation

A simple `email.Contains('@')` check is insufficient. Strings like `"missing-domain@"` and `"@missinglocal.com"` contain `@` but are invalid. Use `IndexOf` to verify content exists on both sides:

```csharp
var atIndex = trimmed.IndexOf('@');
if (atIndex <= 0 || atIndex == trimmed.Length - 1)
    return Fail<Error, string>(Error.New("Email is not valid"));
```

This catches: no `@` (returns `-1`), `@` at start, and `@` at end.

### Marten & NSubstitute

- `IDocumentSession` is a substitutable interface — NSubstitute works without any special setup.
- `Store<T>(params T[] entities)` can be verified with `Arg.Any<T>()`:
  ```csharp
  _session.Received(1).Store(Arg.Any<Lead>());
  ```
- `ThrowsAsync` for async method stubs requires `using NSubstitute.ExceptionExtensions;`.

### .NET 10 / Aspire Deprecations

- `WithOpenApi()` on minimal API endpoints is deprecated in .NET 10 (`ASPDEPR002`). Remove it; OpenAPI is configured at the application level via `MapOpenApi()`.

### Functional Core, Imperative Shell in Practice

- **Validator** = pure function, no dependencies, fully unit-testable without mocks.
- **Handler** = imperative shell that coordinates side effects (Marten). Wrap in try/catch and return `Either<Error, Lead>` so callers never deal with exceptions.
- **Endpoint** = thin orchestrator: validate → handle → map to HTTP result.

This separation means 12 of the 19 tests require zero mocking (pure validator tests), while only 4 handler tests need NSubstitute.

### Nullable Request Fields

Request DTOs should use nullable string properties (`string? Name`) even when the field is logically required. This allows the functional validator to be the single source of truth rather than relying on ASP.NET model binding to reject nulls before validation runs.
