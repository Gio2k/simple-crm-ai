# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## ⚠️ IMPORTANT: Check Plan First

**Before starting any work, always read `Documentation/plan.md`** to understand:
- What has been completed
- What is currently pending
- Known issues and blockers
- The implementation phases

After completing work, **update the plan.md** to reflect progress by marking completed items and adding any new tasks discovered.

## Project Overview

A CRM web application with three components:
- **.NET 10 Web API** - Lead management with CRUD operations, follow-up entries, status tracking, and customer acquisition workflows
- **React/TypeScript Frontend** - Dashboard and editing pages built with Vite
- **Keycloak** - CIAM for authentication/authorization

Local orchestration via .NET Aspire with PostgreSQL (dockerized).

## Architecture

### Web API
- **Domain Driven Design with Vertical Slice Architecture** - Features organized by business capability, not technical layer
- **Functional Core, Imperative Shell** - Pure functions for domain logic, side effects isolated at boundaries
- **language-ext** - Functional programming primitives (Option, Either, Validation, etc.)

### React Frontend
- Functional components only (no class components)
- TypeScript throughout

### Target: 80% test coverage

## Project Structure

```
SimpleCrm.sln
├── src/
│   ├── SimpleCrm.AppHost/           # Aspire orchestration (Postgres, Keycloak, API, Web)
│   ├── SimpleCrm.ServiceDefaults/   # Shared service configuration (health checks, telemetry)
│   ├── SimpleCrm.Api/               # Web API
│   │   ├── Data/                    # DbContext and EF configurations
│   │   └── Features/                # Vertical slices by feature
│   │       └── Leads/Domain/        # Lead and FollowUp entities
│   └── SimpleCrm.Web/               # React frontend (Vite + TypeScript)
└── tests/
    └── SimpleCrm.Api.Tests/         # xUnit + FluentAssertions
```

## Commands

### Development
```bash
# Run entire solution (API + frontend + Keycloak + Postgres)
aspire run

# Run API only
dotnet run --project src/SimpleCrm.Api

# Run frontend only (requires npm install first)
cd src/SimpleCrm.Web && npm run dev

# Build solution
dotnet build

# Run tests
dotnet test

# Run single test file
dotnet test --filter "FullyQualifiedName~ClassName"

# Run single test
dotnet test --filter "FullyQualifiedName=Namespace.ClassName.MethodName"

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"
```

### Frontend Commands
```bash
cd src/SimpleCrm.Web
npm install          # Install dependencies
npm run dev          # Development server
npm run build        # Production build
npm run preview      # Preview production build
npm run lint         # Run ESLint
```

## Vertical Slice Pattern

Each feature is self-contained in its own folder:

```
SimpleCrm.Api/Features/
└── Leads/
    ├── CreateLead/
    │   ├── CreateLeadEndpoint.cs
    │   ├── CreateLeadHandler.cs
    │   ├── CreateLeadRequest.cs
    │   └── CreateLeadValidator.cs
    ├── UpdateLead/
    └── Domain/
        ├── Lead.cs
        └── FollowUp.cs
```

## Functional Patterns with language-ext

Use `Option<T>` instead of null, `Either<L, R>` for operations that can fail, `Validation<Fail, Success>` for accumulating errors:

```csharp
// Domain logic as pure functions
public static Either<Error, Lead> UpdateStatus(Lead lead, LeadStatus newStatus) =>
    lead.Status == LeadStatus.Won
        ? Left<Error, Lead>(new Error("Cannot update won lead"))
        : Right<Error, Lead>(lead with { Status = newStatus });
```

## Keycloak Configuration

Keycloak runs as a container resource in Aspire. Configure realm, clients, and roles for the CRM application. The API validates JWT tokens issued by Keycloak.

## Domain Model

- **Lead**: Name, Email, Phone, Company, Status (New → Contacted → Qualified → Proposal → Negotiation → Won/Lost)
- **FollowUp**: Notes, Type (Call/Email/Meeting/Task/Note), ScheduledFor, IsCompleted
