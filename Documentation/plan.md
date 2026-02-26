# SimpleCRM Development Plan

This document tracks the implementation progress and next steps for the SimpleCRM project.

## Project Status: Foundation Complete

**Last Updated:** 2026-02-01

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
- [ ] **CreateLead** slice
  - [ ] CreateLeadRequest.cs (DTO)
  - [ ] CreateLeadValidator.cs (functional validation with language-ext)
  - [ ] CreateLeadHandler.cs (business logic with language-ext)
  - [ ] CreateLeadEndpoint.cs (minimal API endpoint)
  - [ ] Unit tests

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

### Phase 3: React Frontend ✅
- [x] Install npm dependencies (added react-router-dom)
- [x] Set up React Router (BrowserRouter with Layout route)
- [x] Create layout components (Sidebar, Layout with Outlet)
- [x] **Dashboard page**
  - [x] Lead statistics cards (count per status)
  - [x] Recent leads list
  - [x] Upcoming follow-ups
- [x] **Leads list page**
  - [x] Data table with sorting/filtering
  - [x] Status badges
  - [x] Actions (edit, delete)
- [x] **Lead detail/edit page**
  - [x] Lead information form
  - [x] Follow-up history
  - [x] Add follow-up modal
- [x] **Create lead page** (with validation)
- [x] Mock data layer (`src/api/mockData.ts`, `src/api/leads.ts`) — real API calls not wired

### Phase 4: Database & Document Store
- [x] Marten document store configured
- [ ] Add seed data for development

### Phase 5: Testing & Quality
- [ ] Achieve 80% code coverage target
- [ ] Add integration tests with WebApplicationFactory
- [ ] Add React component tests
- [ ] Configure CI pipeline (optional)

### Phase 6: Playwright E2E Tests

**Approach:** Install `@playwright/test` inside `src/SimpleCrm.Web`. Configure it to auto-start the Vite dev server and run tests against mock data — no real API or backend required.

#### Setup
- [ ] Add `@playwright/test` to devDependencies in `src/SimpleCrm.Web/package.json`
- [ ] Add `"test:e2e": "playwright test"` script
- [ ] Run `npm install` + `playwright install chromium`
- [ ] Create `src/SimpleCrm.Web/playwright.config.ts`
  - `baseURL: http://localhost:5173`
  - `webServer`: `npm run dev`, `reuseExistingServer: true`
  - Chromium only (extend to firefox/webkit later)
- [ ] Update `tsconfig.json` to exclude `e2e/` from Vite's TS build

#### Test files (`src/SimpleCrm.Web/e2e/`)

- [ ] **`dashboard.spec.ts`**
  - Heading "Dashboard" visible
  - 7 status stat cards with correct mock counts
  - Recent leads table shows 5 rows
  - Upcoming follow-ups list rendered
  - Clicking lead name navigates to `/leads/:id`
  - Clicking follow-up lead name navigates to `/leads/:id`

- [ ] **`leads-list.spec.ts`**
  - Heading "Leads" visible; all 8 mock leads shown
  - Search input filters rows by name/company/email
  - Status dropdown filters to matching status
  - Column header click sorts table (toggle asc/desc)
  - "Edit" button navigates to `/leads/:id`
  - "Delete" button: confirm → lead removed from table
  - "+ New Lead" navigates to `/leads/new`

- [ ] **`create-lead.spec.ts`**
  - Heading "New Lead" visible
  - Submitting empty form shows validation errors (Name, Email, Company)
  - Invalid email shows format error
  - Valid submission navigates to new lead's detail page
  - "Cancel" and "← Back to Leads" navigate to `/leads`

- [ ] **`lead-detail.spec.ts`**
  - Form fields pre-populated with lead data
  - Editing fields and saving keeps updated values
  - Status dropdown change updates badge immediately
  - Follow-up list shows existing follow-ups
  - "+ Add" opens modal; submit adds item to list
  - "✓ Complete" marks follow-up completed
  - "Delete" on follow-up removes it from list
  - "← Back to Leads" navigates to `/leads`
  - Unknown lead ID redirects to `/leads`

- [ ] **`navigation.spec.ts`**
  - Sidebar visible on all pages with brand "SimpleCRM"
  - Each sidebar link navigates to correct route
  - Active link has highlighted styling
  - Unknown path redirects to `/`

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
