# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

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

## Project Structure (Aspire-native)

```
SimpleCrm.sln
├── src/
│   ├── SimpleCrm.AppHost/           # Aspire orchestration
│   ├── SimpleCrm.ServiceDefaults/   # Shared service configuration
│   ├── SimpleCrm.Api/               # Web API
│   └── SimpleCrm.Web/               # React frontend
└── tests/
    └── SimpleCrm.Api.Tests/
```

## Commands

### Initial Setup
```bash
# Create Aspire solution
dotnet new aspire -n SimpleCrm
cd SimpleCrm

# Add service defaults
dotnet new aspire-servicedefaults -n SimpleCrm.ServiceDefaults -o src/SimpleCrm.ServiceDefaults

# Add Web API project
dotnet new webapi -n SimpleCrm.Api -o src/SimpleCrm.Api
dotnet sln add src/SimpleCrm.Api

# Add language-ext and testing packages
dotnet add src/SimpleCrm.Api package LanguageExt.Core
dotnet add tests/SimpleCrm.Api.Tests package xunit
dotnet add tests/SimpleCrm.Api.Tests package FluentAssertions

# Create React frontend with Vite
npm create vite@latest SimpleCrm.Web -- --template react-ts
```

### Development
```bash
# Run entire solution (API + frontend + Keycloak + Postgres)
dotnet run --project src/SimpleCrm.AppHost

# Run API only
dotnet run --project src/SimpleCrm.Api

# Run frontend only
cd src/SimpleCrm.Web && npm run dev

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
npm test             # Run tests
```

## Vertical Slice Pattern

Each feature is self-contained in its own folder:

```
SimpleCrm.Api/
└── Features/
    └── Leads/
        ├── CreateLead/
        │   ├── CreateLeadEndpoint.cs
        │   ├── CreateLeadHandler.cs
        │   ├── CreateLeadRequest.cs
        │   └── CreateLeadValidator.cs
        ├── UpdateLead/
        └── Domain/
            └── Lead.cs
```

## Functional Patterns with language-ext

Use `Option<T>` instead of null, `Either<L, R>` for operations that can fail, `Validation<Fail, Success>` for accumulating errors:

```csharp
// Domain logic as pure functions
public static Either<Error, Lead> UpdateStatus(Lead lead, LeadStatus newStatus) =>
    lead.Status == LeadStatus.Closed
        ? Left<Error, Lead>(new Error("Cannot update closed lead"))
        : Right<Error, Lead>(lead with { Status = newStatus });
```

## Keycloak Configuration

Keycloak runs as a container resource in Aspire. Configure realm, clients, and roles for the CRM application. The API validates JWT tokens issued by Keycloak.
