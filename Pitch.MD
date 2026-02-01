# Goal
A simple CRM web application consisting of 3 components:
- A dotnet Web API with the functionality to create leads, update lead information, add and edit follow-up entries and update lead status and finalize customer acquisition
- A React UI Frontend for the API with a dashboard and editing pages
- A Keycloak container resource for enabling CIAM for the CRM application

# Technology
- Latest .net (.net10)
- Latest stable react with typescript
- Postgres as persistence store (dockerized)
- Local orchestration using aspire.dev

# Architectural guidelines
- Functional components for the React Frontend
- Domain Driven Design with Vertical Slice Architecture for the Web API
- Functional approach for the Web API, using the Functional Core, Imperative Shell approach
- Use of the language-ext library in the Web API to enable the functional programming approach
- Target test coverage of 80%

# Other guidelines
- No cloud deployment necessary, just a local showcase application
