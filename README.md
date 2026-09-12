# Library Management System — Backend API

A production-oriented library management backend built with ASP.NET Core,
following a locked clean/onion architecture and a frozen frontend-backend contract.

## Solution Structure

| Project | Responsibility |
|---|---|
| LibraryManagement.Domain | Entities, enums, repository contracts, domain rules |
| LibraryManagement.Shared | DTOs, query parameters, PagedResponse, Result-adjacent contracts |
| LibraryManagement.Services.Abstraction | Service interfaces (contracts for Presentation) |
| LibraryManagement.Services | Business logic, validation, Result mapping, AutoMapper profiles |
| LibraryManagement.Persistence | EF Core, Identity, repositories, JWT/refresh store, MailKit, file storage |
| LibraryManagement.Presentation | Controllers (thin), route & role declarations |
| LibraryManagement.API | Composition root: middleware pipeline, DI, configuration |

Reference rules (enforced by project references):
Domain/Shared → none · Services.Abstraction → Shared · Services → Domain + Abstraction ·
Persistence → Services · Presentation → Abstraction · API → Persistence + Presentation.

## Key Patterns

- **Result<T>** everywhere in services; controllers map failures to frozen Problem Details.
- **Unit of Work + Repository** with tracked/untracked switches.
- **Optimistic concurrency** via SQL Server `rowversion` on Books
  (400 missing / 409 stale on update; 409 on borrow races).
- **Auth**: JWT access token (15 min) + HttpOnly refresh cookie scoped to `/api/v1/auth`,
  with rotation and revocation on logout/reset.
- **Error contract**: RFC 9457 Problem Details with `traceId` on every error path
  (Result failures, ModelState 400s, rate-limit 429, unexpected 500).
- **Rate limiting**: global per-IP + stricter auth policy, `Retry-After` exposed via CORS.
- **API versioning**: all routes under `/api/v1`.

## Getting Started

See [Docs/SETUP.md](Docs/SETUP.md) for prerequisites, secrets, migrations, seeding and run steps.

## Testing

Import `Docs/LibraryManagementAPI.postman_collection.json` with
`Docs/LibraryManagementAPI.postman_environment.json`, then run folders 00 → 06 in order.
Details and expected results: [Docs/TESTING.md](Docs/TESTING.md).

## Documentation Index

- [Architecture](Docs/ARCHITECTURE.md)
- [API Contract (as-built)](Docs/API-CONTRACT.md)
- [Security](Docs/SECURITY.md)
- [Setup](Docs/SETUP.md)
- [Testing](Docs/TESTING.md)
- [Deployment](Docs/DEPLOYMENT.md)
- Frozen frontend contract: `Frontend-Backend-API-Contract.md`
