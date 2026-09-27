# Implementation Summary — Create Lead (Phase 1)

**Date:** 2026-07-02
**Scope:** `CreateLead` vertical slice (Phase 1 – Leads Feature)
**Status:** Complete — builds cleanly, 12/12 unit tests passing

---

## 1. What was implemented

### New files

| File | Responsibility |
|---|---|
| `src/SimpleCrm.Api/Features/Leads/CreateLead/CreateLeadRequest.cs` | Inbound DTO (`CreateLeadRequest`, nullable props) + immutable post-validation record (`ValidatedCreateLeadRequest`) |
| `src/SimpleCrm.Api/Features/Leads/CreateLead/CreateLeadValidator.cs` | Functional validation with language-ext `Validation<CreateLeadValidationError, ValidatedCreateLeadRequest>`; errors accumulate via tuple `.Apply` |
| `src/SimpleCrm.Api/Features/Leads/CreateLead/CreateLeadHandler.cs` | Use case: pure `CreateLead()` (domain logic) + `HandleAsync()` returning `Either<CreateLeadError, Lead>` (imperative shell) |
| `src/SimpleCrm.Api/Features/Leads/CreateLead/CreateLeadEndpoint.cs` | Minimal API endpoint mapping `POST /api/leads` onto the functional pipeline |
| `src/SimpleCrm.Api/Data/Leads/ILeadRepository.cs` | Persistence port (enables handler unit tests without a live database) |
| `src/SimpleCrm.Api/Data/Leads/MartenLeadRepository.cs` | Marten-backed adapter over the scoped `IDocumentSession` |
| `tests/SimpleCrm.Api.Tests/Features/Leads/CreateLead/CreateLeadValidatorTests.cs` | 9 validator tests |
| `tests/SimpleCrm.Api.Tests/Features/Leads/CreateLead/CreateLeadHandlerTests.cs` | 3 handler tests |

### Modified files

| File | Change |
|---|---|
| `src/SimpleCrm.Api/Program.cs` | Registers `ILeadRepository` → `MartenLeadRepository` (scoped); maps `app.MapCreateLeadEndpoint()` |
| `src/SimpleCrm.Api/SimpleCrm.Api.http` | Added example `POST /api/leads` request |
| `Documentation/plan.md` | CreateLead slice marked complete; added repository port/adapter to the checklist |

---

## 2. Design

- **Vertical slice + functional core / imperative shell** per `CLAUDE.md`:
  - Pure function `CreateLeadHandler.CreateLead(ValidatedCreateLeadRequest)` constructs a `Lead` with `Status = New`, new `Id`, and `DateTime.UtcNow` timestamps.
  - Repository persistence is the isolated side effect, wrapped in `Either<CreateLeadError, Lead>`.
- **Validation:** all rules run and errors accumulate:
  - Required: `Name`, `Email` (missing/whitespace → error)
  - Format: `Email` must match a light regex, ≤ 254 chars
  - Length: `Name` ≤ 100, `Phone` ≤ 50, `Company` ≤ 150
  - Optional fields normalized: blank → `null`, otherwise trimmed
- **HTTP mapping** (`POST /api/leads`):
  - `201 Created` + lead, with `Location: /api/leads/{id}`
  - `400 Bad Request` with `ValidationProblem` errors keyed by property
  - `500 Internal Server Error` if persistence fails

---

## 3. API design notes / gotchas discovered

1. **`.Apply` idiom (language-ext 4.4.9):** the CLAUDE.md pattern
   `(ValidateName(...), ValidateEmail(...), ...).Apply((...) => ...)` works **only in tuple form**. The sequential
   `Success<F, Func<...>>().Apply(...).Apply(...)` chain does not exist at this package version for 3+ argument functions
   (verified via reflection on `ValidationSeqExtensions`).

2. **language-ext null strictness:** `Validation.Success<T>(null)` throws `ValueIsNullException`, and
   `Option<string>.Match(...)` returning `null` throws `ResultIsNullException`. Workaround: optional fields use an
   empty-string sentinel that is mapped back to `null` in the final validator mapper (`EmptyToNull`).

3. **`MatchAsync` nesting:** an `async` lambda that returns `Task<IResult>` (un-awaited) does not type-check against
   `Func<T, Task<RESULT>>`. The endpoint awaits the inner `Either` first and delegates the success path to a separate
   helper method (`HandleValidatedAsync`).

4. **Request DTO nullability:** DTO fields are intentionally `string?` so missing properties flow through the
   validator and produce friendly accumulating error messages (rather than an automatic JSON 400).

---

## 4. Test coverage (unit)

| Area | # Tests | Cases |
|---|---|---|
| `CreateLeadValidatorTests` | 9 | valid request + trimming, optional-blank → null, missing name, accumulating name + email errors, invalid email format, name/phone/company length limits |
| `CreateLeadHandlerTests` | 3 | pure `CreateLead` construction, persistence success path (Right + stored lead), persistence failure path (Left error) |

Handler persistence is tested via a hand-rolled `FakeLeadRepository` implementing the `ILeadRepository` port.
A real Marten/Postgres round-trip is deferred to the Phase 5 integration-test milestone (`WebApplicationFactory`).

---

## 5. Verification

```bash
dotnet build   # Build succeeded
dotnet test    # Passed: 12, Failed: 0, Skipped: 0
```

---

## 6. Not addressed (future phases)

- Keycloak auth / endpoint protection (Phase 2)
- Remaining Lead slices: GetLeads, GetLeadById, UpdateLead, UpdateLeadStatus, DeleteLead (Phase 1)
- FollowUps feature (Phase 1)
- Seed data (Phase 4), 80% coverage / integration tests / CI (Phase 5), Playwright E2E (Phase 6)
- Pre-existing NU1902/NU1903 package vulnerability warnings in AppHost/Api are unrelated to this change