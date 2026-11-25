# Library Management System — Backend API

A production-oriented library management backend built with ASP.NET Core (.NET 10), following a locked clean/onion architecture and a frozen frontend–backend contract.

The application supports containerized local development using Docker and Docker Compose, with SQL Server, persistent storage, and Mailpit for email testing.

## Live Demo

The Library Management API is publicly deployed on MonsterASP.NET using ASP.NET Core (.NET 10), IIS, and SQL Server 2025.

| Resource | Link |
|---|---|
| Live Swagger UI | [Explore the API](https://library-management-api.runasp.net/swagger/index.html) |
| OpenAPI Specification | [Swagger JSON](https://library-management-api.runasp.net/swagger/v1/swagger.json) |
| Liveness | [Application Health](https://library-management-api.runasp.net/health) |
| Readiness | [Database Health](https://library-management-api.runasp.net/health/ready) |

> **Portfolio demo:** The application uses a free hosting plan with limited resources and no uptime guarantee. Email delivery is unavailable in the hosted environment; email confirmation and password reset can be tested locally using Mailpit. Use fictional test data only.

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
2. Swagger: `https://localhost:7113/swagger` · Liveness: `https://localhost:7113/health` · Readiness: `https://localhost:7113/health/ready`.
3. Contract test suite: import the Postman collection + environment from `Docs/` and follow [Docs/TESTING.md](Docs/TESTING.md).

### Option 2 — Docker Development

Run the backend API, SQL Server, and Mailpit using Docker Compose.

Follow [Docs/DOCKER.md](Docs/DOCKER.md) for environment configuration, secret generation, network and volume initialization, database migrations, and startup instructions.

## Docker Support

The application provides a containerized local development environment with the following features:

- **Multi-stage Docker build** using .NET 10 SDK and ASP.NET Core Runtime images.
- **Docker Compose** for managing application services.
- **SQL Server 2022** with persistent database storage.
- **Database health checks** to ensure the SQL Server engine accepts connections before starting the API.
- **Docker networking** for service-to-service communication.
- **Named volumes** for persistent application data.
- **Environment-based configuration** with secret-free configuration templates.
- **Mailpit integration** for local email confirmation and password reset testing.
- **Docker build caching** through separate dependency restoration and application publishing stages.
- **Docker build context optimization** using `.dockerignore`.
- **Non-root container execution** using a dedicated application user (`app`).
- **Application health checks** with separate liveness and database readiness endpoints.
- **Environment-specific logging** using Serilog.

### Docker Services

| Service | Container | Local Address |
|---|---|---|
| ASP.NET Core API | `library-api-test` | `http://localhost:8080` |
| SQL Server 2022 | `library-sqlserver` | `localhost:14333` |
| Mailpit | `library-mailpit` | `http://localhost:8025` |

**Swagger UI:** [http://localhost:8080/swagger/index.html](http://localhost:8080/swagger/index.html)

**Mailpit Inbox:** [http://localhost:8025](http://localhost:8025)

**Liveness Check:** [http://localhost:8080/health](http://localhost:8080/health)

**Readiness Check:** [http://localhost:8080/health/ready](http://localhost:8080/health/ready)

### Persistent Storage

| Volume | Container Path | Purpose |
|---|---|---|
| `library-sql-data` | `/var/opt/mssql` | SQL Server database persistence |
| `library-book-images` | `/app/wwwroot/uploads/books` | Uploaded book cover images |
| `library-dp-keys` | `/home/app/.aspnet/DataProtection-Keys` | ASP.NET Core Data Protection keys |

Named volumes preserve application data across container recreation.

Existing volumes created before the non-root container configuration may require a one-time ownership migration. See [Docs/DOCKER.md](Docs/DOCKER.md) for instructions.

### Environment Configuration

| File | Purpose |
|---|---|
| `.env.example` | API configuration template |
| `.env.sqlserver.example` | SQL Server configuration template |
| `.env` | Local API configuration and secrets (Git ignored) |
| `.env.sqlserver` | Local SQL Server configuration and credentials (Git ignored) |
| `LibraryManagement.API/appsettings.Production.json` | Non-sensitive production configuration defaults |

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
- Non-root container execution with UID 1654.
- Writable directories for book covers, Data Protection keys, and development logs.
- Successful migration of existing volume ownership.
- Application liveness and database readiness health checks.

> **Note:** The provided Docker Compose configuration is intended for local development and testing. Public deployment requires separate infrastructure, HTTPS, secure secret management, and appropriate production configuration.

## Production Readiness

The API includes production-oriented configuration and security features:

- **Environment-specific configuration** through `appsettings.Production.json`.
- **Configurable Swagger** availability, disabled in Production by default.
- **Configurable CORS** allowed origins.
- **Forwarded headers** support with configurable trusted reverse proxy networks.
- **JWT signing key validation** with a minimum length of 32 bytes for HMAC SHA256.
- **JWT token lifetime validation** during application startup.
- **Database connection string validation** using a fail-fast approach.
- **Authentication error hygiene** with sensitive JWT error details disabled.
- **SMTP security** preventing development-only local SMTP from being enabled in Production.
- **Configurable Data Protection key storage** for different hosting environments.
- **Environment-specific exception handling** without exposing internal exception details in Production.
- **Console-based production logging** using Serilog, with file logging restricted to Development.
- **Non-root Docker container execution** with dedicated writable directories.
- **Separate liveness and readiness health checks** for application monitoring.

### Health Check Endpoints

| Method | Endpoint | Purpose |
|---|---|---|
| GET | `/health` | Application liveness |
| GET | `/health/ready` | Database connectivity check |

The liveness endpoint verifies that the application is responsive without querying the database.

The readiness endpoint verifies database connectivity through an Entity Framework Core health check. It does not validate the entire database schema or verify all application dependencies.

### Production Configuration

Production configuration can be supplied through environment variables without storing sensitive information in source control.

| Configuration Key | Purpose |
|---|---|
| `ASPNETCORE_ENVIRONMENT` | Application environment |
| `Swagger__Enabled` | Controls Swagger availability in Production |
| `ConnectionStrings__DefaultConnection` | Database connection string |
| `Jwt__SecretKey` | JWT signing key |
| `Jwt__Issuer` | JWT issuer |
| `Jwt__Audience` | JWT audience |
| `Jwt__AccessTokenExpirationMinutes` | Access token lifetime |
| `Jwt__RefreshTokenExpirationDays` | Refresh token lifetime |
| `Cors__AllowedOrigins__0` | Allowed frontend origin |
| `ReverseProxy__Enabled` | Enables forwarded headers processing |
| `ReverseProxy__KnownNetworks__0` | Trusted reverse proxy network |
| `DataProtection__KeysPath` | Custom Data Protection key storage path |
| `EmailOptions__UseLocalSmtp` | Development-only local SMTP mode |

Production credentials must be generated separately from local development credentials.

### Production Verification

The application has been smoke-tested locally in the Production environment with:

- Swagger explicitly enabled.
- Development-only local SMTP disabled.
- Production configuration and startup validation enabled.
- Successful application startup.
- Successful liveness and database readiness checks.

The public deployment was verified on November 25, 2026:

- Public HTTPS access using a Let's Encrypt certificate and HTTP-to-HTTPS redirection configured at the hosting layer.
- Framework-dependent Release publishing for .NET 10 and manual deployment over SFTP.
- Production configuration supplied through hosting environment variables, with separate deployment credentials.
- SQL Server 2025 database provisioned separately from local development, with both existing EF Core migrations applied and three predefined roles seeded.
- Live Swagger UI and OpenAPI document available.
- Application liveness and database readiness endpoints returned `Healthy`.
- `GET /api/v1/books` smoke test completed successfully.

The hosted deployment runs on IIS, while Docker Compose remains the local development and testing workflow. Hosted email delivery, full hosted regression testing, and automated CI/CD deployment have not been verified or implemented as part of this release.

For production deployment requirements and security guidance, see [Docs/DEPLOYMENT.md](Docs/DEPLOYMENT.md) and [Docs/SECURITY.md](Docs/SECURITY.md).

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