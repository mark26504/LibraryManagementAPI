# Deployment Guide

## Current Target

Development/local only: Kestrel on `https://localhost:7113`, SQL Server (LocalDB or full),
local `wwwroot/uploads` storage, Gmail SMTP, Angular on `localhost:4200`.
No production CI/CD pipeline exists yet — this document defines the production-readiness bar.

## Configuration Surface

| Setting | Dev source | Production requirement |
|---|---|---|
| `ConnectionStrings:DefaultConnection` | User Secrets | Environment variable / secret manager |
| JWT signing key, issuer, audience | User Secrets | Secret manager + key rotation plan |
| SMTP host/port/user/password | User Secrets | Managed email provider credentials |
| File storage folder whitelist & limits | appsettings | Same + mirrored reverse-proxy body limits |
| CORS origins | appsettings (`localhost:4200`) | Explicit allow-list of real origins only |

## HTTPS & Cookies

- Dev: `dotnet dev-certs https --trust`; Kestrel serves HTTPS directly.
- Production: TLS termination at the reverse proxy, HSTS enabled.
- The refresh cookie keeps its `/api/v1/auth` scope and must carry `Secure` + `SameSite`
  per environment configuration.

## Database

```bash
dotnet ef database update --project LibraryManagement.Persistence --startup-project LibraryManagement.API
```

- Migrations are the only schema path; no manual SQL in deployments.
- Role/demo-account seeding is **development-only** — verify it is disabled in production builds.

## Static Uploads

- Book covers live under `wwwroot/uploads/books` and are served by the static-files middleware.
- Production option: object storage/CDN behind the same URL shape
  (`/uploads/books/{guid}.{ext}`); filenames remain server-generated Guids only.

## Observability

- Serilog (console + rolling file); every log carries the request traceId.
- `GET /health` is the liveness/readiness probe for any orchestrator or load balancer.
- Problem Details `traceId` is the correlation key between clients and logs.

## Security Gates Before Any Production Run

1. Swagger disabled or admin-protected.
2. Demo seeding off; seeded passwords removed.
3. JWT key ≥ 256-bit random, stored in a secret store, rotation documented.
4. Rate-limit thresholds reviewed against expected traffic.
5. CORS allow-list contains only real frontend origins.
6. Upload size/extension limits mirrored at the reverse proxy.
7. Error responses verified to be Problem Details only (no stack traces).
8. Refresh cookie `Secure=true`.

## Release Checklist (per release)

- Postman suite folders 00 → 06 fully green (`Docs/TESTING.md`).
- Manual gate scenarios green: rowVersion matrix, final-copy race,
  refresh rotation & revocation, email-token single use.
- Documentation refreshed against as-built behavior.
- Version tag applied per the release gate decision.
