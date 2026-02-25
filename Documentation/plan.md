# SimpleCRM Development Plan

This document tracks the implementation progress and next steps for the SimpleCRM project.

## Project Status: Foundation Complete

**Last Updated:** 2026-02-26

---

## Completed Work

### 1. Project Structure Setup ✅
- [x] Created Aspire-native solution structure
- [x] Organized into `src/` and `tests/` folders
- [x] Solution file configured with proper project references

### 2. Backend Projects ✅
- [x] **SimpleCrm.AppHost** - Aspire orchestration configured with:
  - PostgreSQL with PgAdmin
  - Keycloak (preview package)
  - API project reference
  - React frontend (npm app)
- [x] **SimpleCrm.ServiceDefaults** - Health checks and telemetry
- [x] **SimpleCrm.Api** - Web API with:
  - ServiceDefaults integration
  - Marten document store registration
  - OpenAPI support

### 3. Domain Model ✅
- [x] **Lead** entity (`Features/Leads/Domain/Lead.cs`)
  - Properties: Id, Name, Email, Phone, Company, Status, CreatedAt, UpdatedAt
  - Status enum: New, Contacted, Qualified, Proposal, Negotiation, Won, Lost
- [x] **FollowUp** entity (`Features/Leads/Domain/FollowUp.cs`)
  - Properties: Id, LeadId, Notes, Type, CreatedAt, ScheduledFor, IsCompleted
  - Type enum: Call, Email, Meeting, Task, Note
- [x] **Marten document store** configured as document database

### 4. Frontend Setup ✅
- [x] **SimpleCrm.Web** - Vite + React + TypeScript scaffolding
  - package.json with dependencies
  - TypeScript configuration
  - Vite configuration with API proxy
  - ESLint configuration
  - Basic App.tsx placeholder

### 5. Test Project ✅
- [x] **SimpleCrm.Api.Tests** - xUnit project with AwesomeAssertions
- [x] Project reference to API
- [x] Tests pass (1 placeholder test)

### 6. Packages Installed ✅
- [x] API: LanguageExt.Core, Marten, Marten.AspNetCore
- [x] AppHost: Aspire.Hosting.PostgreSQL, Aspire.Hosting.Keycloak, Aspire.Hosting.NodeJs
- [x] Tests: AwesomeAssertions

### 7. Documentation ✅
- [x] CLAUDE.md created with project guidance
- [x] .gitignore configured

---

## Pending Work

### Phase 1: API Endpoints (Vertical Slices)

#### Leads Feature
- [x] **CreateLead** slice
  - [x] CreateLeadRequest.cs (DTO)
  - [x] CreateLeadValidator.cs (functional validation with language-ext)
  - [x] CreateLeadHandler.cs (business logic with language-ext)
  - [x] CreateLeadEndpoint.cs (minimal API endpoint)
  - [x] Unit tests (19 tests: validator + handler with NSubstitute mocking)

- [ ] **GetLeads** slice (list with filtering/pagination)
  - [ ] GetLeadsRequest.cs
  - [ ] GetLeadsHandler.cs
  - [ ] GetLeadsEndpoint.cs
  - [ ] Unit tests

- [ ] **GetLeadById** slice
  - [ ] GetLeadByIdHandler.cs
  - [ ] GetLeadByIdEndpoint.cs
  - [ ] Unit tests

- [ ] **UpdateLead** slice
  - [ ] UpdateLeadRequest.cs
  - [ ] UpdateLeadValidator.cs
  - [ ] UpdateLeadHandler.cs
  - [ ] UpdateLeadEndpoint.cs
  - [ ] Unit tests

- [ ] **UpdateLeadStatus** slice
  - [ ] UpdateLeadStatusRequest.cs
  - [ ] UpdateLeadStatusHandler.cs (with status transition rules)
  - [ ] UpdateLeadStatusEndpoint.cs
  - [ ] Unit tests

- [ ] **DeleteLead** slice
  - [ ] DeleteLeadHandler.cs
  - [ ] DeleteLeadEndpoint.cs
  - [ ] Unit tests

#### FollowUps Feature
- [ ] **AddFollowUp** slice
- [ ] **GetFollowUps** slice (by lead)
- [ ] **UpdateFollowUp** slice
- [ ] **CompleteFollowUp** slice
- [ ] **DeleteFollowUp** slice

### Phase 2: Authentication & Authorization
- [ ] Configure Keycloak realm for SimpleCRM
- [ ] Create Keycloak client for API
- [ ] Create Keycloak client for React SPA
- [ ] Add JWT authentication to API
- [ ] Implement role-based authorization (Admin, Sales, Viewer)
- [ ] Protect API endpoints

### Phase 3: React Frontend
- [ ] Install npm dependencies
- [ ] Set up React Router
- [ ] Create layout components (Header, Sidebar, Footer)
- [ ] **Dashboard page**
  - [ ] Lead statistics cards
  - [ ] Recent leads list
  - [ ] Upcoming follow-ups
- [ ] **Leads list page**
  - [ ] Data table with sorting/filtering
  - [ ] Status badges
  - [ ] Actions (edit, delete)
- [ ] **Lead detail/edit page**
  - [ ] Lead information form
  - [ ] Follow-up history
  - [ ] Add follow-up modal
- [ ] **Create lead page**
- [ ] Authentication integration (Keycloak JS adapter)

### Phase 4: Database & Document Store
- [x] Marten document store configured
- [ ] Add seed data for development

### Phase 5: Testing & Quality
- [ ] Achieve 80% code coverage target
- [ ] Add integration tests with WebApplicationFactory
- [ ] Add React component tests
- [ ] Configure CI pipeline (optional)

---

## Open Topics (For Refinement)

The following items were identified during a plan review and need discussion before implementation.

### Inconsistencies

1. **CLAUDE.md references stale `Data/` directory** — The project structure in CLAUDE.md documents `src/SimpleCrm.Api/Data/` as containing "DbContext and EF configurations." This directory does not exist since the migration to Marten (commit `d7298b9`). CLAUDE.md should be updated to remove or replace this reference.

2. **AppHost "React frontend (npm app)" wording** — Section 2 states AppHost has a React frontend project reference, but the `.csproj` only contains a project reference to `SimpleCrm.Api`. The npm app is likely configured in `Program.cs` code, not as a project reference. Clarify wording.

3. **Placeholder test `UnitTest1.cs` still exists** — Listed in Section 5 as "1 placeholder test" but never marked for cleanup. Should be removed now that real tests exist.

### Undefined Requirements

4. **GetLeads filtering and pagination** — What filters should be supported? (by status, company, date range, name search?) What pagination model? (offset/limit vs cursor-based) What default sort order?

5. **UpdateLeadStatus transition rules** — The plan mentions "status transition rules" but they are not defined. Decisions needed:
   - Is the flow strictly linear (New → Contacted → Qualified → Proposal → Negotiation → Won/Lost)?
   - Can statuses be skipped (e.g., New → Qualified)?
   - Can a Lost lead be reopened?
   - Can a Won lead be modified?

6. **DeleteLead — soft delete or hard delete?** — Not specified. For a CRM, soft delete is typical to preserve audit history, but this needs a decision. If soft delete, should there be a restore endpoint?

7. **FollowUp slices lack detail** — All five FollowUp slices are single bullet points with no sub-task breakdown (Request/Validator/Handler/Endpoint/Tests), unlike the Lead slices. These should be expanded before implementation.

8. **Keycloak role-to-endpoint mapping** — Three roles are listed (Admin, Sales, Viewer) but there is no definition of which roles can access which endpoints. For example:
   - Can Viewers create/edit leads or only read?
   - Can Sales users delete leads?
   - Are there any data-level restrictions (e.g., Sales users only see their own leads)?

9. **Integration test strategy for Marten/PostgreSQL** — Phase 5 mentions "integration tests with WebApplicationFactory" but does not specify how the PostgreSQL/Marten dependency will be handled in tests (Testcontainers, in-memory alternative, or shared test database).

### Minor Fixes

10. **Quick Start command `aspire run`** — The correct command is likely `dotnet run --project src/SimpleCrm.AppHost`. Verify and update.

---

## Known Issues / Blockers

None currently.

## Resolved Issues

1. ~~**npm not available** - Node.js/npm needs to be installed to run the React frontend~~ - Resolved (npm available)
2. **Vite config for Aspire** - Fixed vite.config.ts to use PORT env variable and listen on all interfaces for Aspire integration

---

## Environment Requirements

- .NET 10 SDK
- Node.js 18+ (for React frontend)
- Docker Desktop (for PostgreSQL and Keycloak containers)

---

## Quick Start Commands

```bash
# Build solution
dotnet build

# Run tests
dotnet test

# Run full stack (requires Docker)
aspire run

# Run API only
dotnet run --project src/SimpleCrm.Api

# Run frontend (after npm install)
cd src/SimpleCrm.Web && npm install && npm run dev
```
