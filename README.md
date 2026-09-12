# Library Management System — Backend API

A production-oriented library management backend built with ASP.NET Core,
following a locked clean/onion architecture and a frozen frontend–backend contract.

## Solution Structure

| Project | Responsibility |
|---|---|
| LibraryManagement.Domain | Entities, enums, repository contracts, domain rules |
| LibraryManagement.Shared | DTOs, query parameters, `PagedResponse<T>` |
| LibraryManagement.Services.Abstraction | Service & infrastructure contracts |
| LibraryManagement.Services | Business logic, Result mapping, AutoMapper profiles |
| LibraryManagement.Persistence | EF Core, Identity, repositories, JWT/refresh store, MailKit, file storage |
| LibraryManagement.Presentation | Thin controllers: routes, roles, binding → Problem Details |
| LibraryManagement.API | Composition root: pipeline, DI, configuration, middleware |

Reference rules are enforced by the project graph; `Services` carries no
`Microsoft.AspNetCore.App` framework reference (compiler-enforced boundary).

## Key Patterns

- **Result / Result<T>** in every service; controllers map failures to frozen Problem Details.
- **Unit of Work + Repository** with explicit tracking switches and include ownership.
- **Optimistic concurrency** via SQL Server `rowversion` on Books
  (400 missing / 409 stale; races → 409).
- **Inventory invariant**: `availableCopies = totalCopies − activeBorrowed`, server-owned.
- **Auth**: JWT access token (15 min) + HttpOnly refresh cookie scoped to `/api/v1/auth`,
  rotated on refresh, revoked on logout/reset/disable.
- **Error contract**: RFC 9457 Problem Details with `traceId` on every error path,
  camelCase validation keys.
- **Rate limiting**: global per-IP + stricter auth policy; `Retry-After` exposed via CORS.
- **Versioning**: all routes under `/api/v1`.

## Quick Start

1. Prerequisites, secrets, migrations, seeding and run steps: [Docs/SETUP.md](Docs/SETUP.md)
2. Swagger: `https://localhost:7113/swagger` · Health: `https://localhost:7113/health`
3. Contract test suite: import the Postman collection + environment from `Docs/`
   and follow [Docs/TESTING.md](Docs/TESTING.md)

## Documentation Index

| Document | Content |
|---|---|
| [Docs/Architecture.md](Docs/Architecture.md) | Layers, reference rules, patterns, DI map |
| [Docs/API-Endpoints.md](Docs/API-Endpoints.md) | As-built endpoint reference (roles, codes) |
| [Docs/BusinessRules.md](Docs/BusinessRules.md) | As-built business rules |
| [Docs/SECURITY.md](Docs/SECURITY.md) | AuthN/AuthZ, email flows, uploads, error hygiene |
| [Docs/SETUP.md](Docs/SETUP.md) | Local setup & troubleshooting |
| [Docs/TESTING.md](Docs/TESTING.md) | Postman suite & manual gate scenarios |
| [Docs/DEPLOYMENT.md](Docs/DEPLOYMENT.md) | Production-readiness bar & release checklist |
| [Docs/ERD.md](Docs/ERD.md) · [Docs/Requirements.md](Docs/Requirements.md) | Design history |
| `Frontend-Backend-API-Contract.md` | The frozen frontend–backend contract |
