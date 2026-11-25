# Library Management System — Backend API

A production-oriented library management backend built with ASP.NET Core (.NET 10), following a locked clean/onion architecture and a frozen frontend–backend contract.

The application supports containerized local development using Docker and Docker Compose, with SQL Server, persistent storage, and Mailpit for email testing.

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

Reference rules are enforced by the project graph; `Services` carries no `Microsoft.AspNetCore.App` framework reference (compiler-enforced boundary).

## Key Patterns

- **Result / Result<T>** in every service; controllers map failures to frozen Problem Details.
- **Unit of Work + Repository** with explicit tracking switches and include ownership.
- **Optimistic concurrency** via SQL Server `rowversion` on Books (400 missing / 409 stale; races → 409).
- **Inventory invariant**: `availableCopies = totalCopies − activeBorrowed`, server-owned.
- **Auth**: JWT access token (15 min) + HttpOnly refresh cookie scoped to `/api/v1/auth`, rotated on refresh, revoked on logout/reset/disable.
- **Error contract**: RFC 9457 Problem Details with `traceId` on every error path, camelCase validation keys.
- **Rate limiting**: global per-IP + stricter auth policy; `Retry-After` exposed via CORS.
- **Versioning**: all routes under `/api/v1`.

## Quick Start

### Option 1 — Local Development

1. Prerequisites, secrets, migrations, seeding and run steps: [Docs/SETUP.md](Docs/SETUP.md).
2. Swagger: `https://localhost:7113/swagger` · Health: `https://localhost:7113/health`.
3. Contract test suite: import the Postman collection + environment from `Docs/` and follow [Docs/TESTING.md](Docs/TESTING.md).

### Option 2 — Docker Development

Run the backend API, SQL Server, and Mailpit using Docker Compose.

Follow [Docs/DOCKER.md](Docs/DOCKER.md) for environment configuration, secret generation, network and volume initialization, database migrations, and startup instructions.

## Docker Support

The application provides a containerized local development environment with the following features:

- **Multi-stage Docker build** using .NET 10 SDK and ASP.NET Core Runtime images.
- **Docker Compose** for managing application services.
- **SQL Server 2022** with persistent database storage.
- **Database health checks** to ensure SQL Server is ready before starting the API.
- **Docker networking** for service-to-service communication.
- **Named volumes** for persistent application data.
- **Environment-based configuration** with secret-free configuration templates.
- **Mailpit integration** for local email confirmation and password reset testing.
- **Docker build caching** through separate dependency restoration and application publishing stages.
- **Docker build context optimization** using `.dockerignore`.

### Docker Services

| Service | Container | Local Address |
|---|---|---|
| ASP.NET Core API | `library-api-test` | `http://localhost:8080` |
| SQL Server 2022 | `library-sqlserver` | `localhost:14333` |
| Mailpit | `library-mailpit` | `http://localhost:8025` |

**Swagger UI:** [http://localhost:8080/swagger/index.html](http://localhost:8080/swagger/index.html)

**Mailpit Inbox:** [http://localhost:8025](http://localhost:8025)

### Persistent Storage

| Volume | Container Path | Purpose |
|---|---|---|
| `library-sql-data` | `/var/opt/mssql` | SQL Server database persistence |
| `library-book-images` | `/app/wwwroot/uploads/books` | Uploaded book cover images |
| `library-dp-keys` | `/root/.aspnet/DataProtection-Keys` | ASP.NET Core Data Protection keys |

Named volumes preserve application data across container recreation.

### Environment Configuration

| File | Purpose |
|---|---|
| `.env.example` | API configuration template |
| `.env.sqlserver.example` | SQL Server configuration template |
| `.env` | Local API configuration and secrets (Git ignored) |
| `.env.sqlserver` | Local SQL Server configuration and credentials (Git ignored) |

Real credentials and signing keys must never be committed to source control.

### Docker Verification

The following workflows have been manually verified in the containerized development environment:

- Database connectivity and EF Core migrations.
- User registration and JWT authentication.
- Role-based authorization for Member and Librarian accounts.
- Category, author, and book creation.
- Book cover upload and static file serving.
- Image persistence after container recreation.
- ASP.NET Core Data Protection key persistence.
- SMTP email delivery using Mailpit.
- Email confirmation and database verification.
- Forgot password and password reset.
- Successful authentication using the updated password.

> **Note:** The provided Docker Compose configuration is intended for local development and testing. Production deployment requires separate infrastructure, HTTPS, secure secret management, and production SMTP configuration.

## Documentation Index

| Document | Content |
|---|---|
| [Docs/Architecture.md](Docs/Architecture.md) | Layers, reference rules, patterns, DI map |
| [Docs/API-Endpoints.md](Docs/API-Endpoints.md) | As-built endpoint reference (roles, codes) |
| [Docs/BusinessRules.md](Docs/BusinessRules.md) | As-built business rules |
| [Docs/SECURITY.md](Docs/SECURITY.md) | AuthN/AuthZ, email flows, uploads, error hygiene |
| [Docs/SETUP.md](Docs/SETUP.md) | Local setup & troubleshooting |
| [Docs/DOCKER.md](Docs/DOCKER.md) | Docker setup, configuration, networking, volumes & troubleshooting |
| [Docs/TESTING.md](Docs/TESTING.md) | Postman suite & manual gate scenarios |
| [Docs/DEPLOYMENT.md](Docs/DEPLOYMENT.md) | Production-readiness bar & release checklist |
| [Docs/ERD.md](Docs/ERD.md) · [Docs/Requirements.md](Docs/Requirements.md) | Design history |
| `Frontend-Backend-API-Contract.md` | The frozen frontend–backend contract |