# Setup Guide

## Prerequisites

- Visual Studio 2022 (17.8+) or the .NET SDK with CLI tooling
- SQL Server 2019+ (LocalDB is sufficient for development)
- Node.js 18+ — only needed when running the Angular frontend
- An SMTP account for development email flows (Gmail app-password works);
  without it, email endpoints still respond per contract and failures appear in logs

## 1. Clone & Restore

```bash
git clone <repo-url>
cd LibraryManagementAPI
dotnet restore LibraryManagement.sln
```

## 2. Secrets (User Secrets on the API project)

Right-click `LibraryManagement.API` → *Manage User Secrets* and provide:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=LibraryManagement;Trusted_Connection=True;TrustServerCertificate=True"
  },
  "Jwt": {
    "Key": "<long-random-secret>",
    "Issuer": "LibraryManagement.API",
    "Audience": "LibraryManagement.Client",
    "AccessTokenMinutes": 15
  },
  "Email": {
    "Host": "smtp.gmail.com",
    "Port": 587,
    "UserName": "<dev-account@gmail.com>",
    "Password": "<app-password>",
    "From": "<dev-account@gmail.com>"
  }
}
```

> Key paths must match the options classes bound at startup
> (JWT options in `Persistence/Authentication`, `EmailOptions` in `Persistence/Email`,
> `FileStorageOptions.SectionName = "FileStorage"`).
> If a binding error appears on first run, align the JSON paths with those classes.
> No credentials ever live in `appsettings.json` or the repository.

## 3. Database

```bash
dotnet ef database update --project LibraryManagement.Persistence --startup-project LibraryManagement.API
```

On first run the application seeds roles, demo accounts and sample catalog data.

**Development accounts (dev-only — never ship seeding to shared environments):**

| Email | Role | Password |
|---|---|---|
| Admin@gmail.com | Admin | Password123! |
| Librarian@gmail.com | Librarian | Password123! |
| Member@gmail.com | Member | Password123! |
| member2@gmail.com | Member | Password123! |

## 4. Run the API

```bash
dotnet run --project LibraryManagement.API
```

- HTTPS: `https://localhost:7113`
- Swagger: `https://localhost:7113/swagger`
- Health: `https://localhost:7113/health`

Trust the development certificate once per machine:

```bash
dotnet dev-certs https --trust
```

## 5. Frontend (optional until integration phase)

The Angular app runs on `http://localhost:4200` with `useMockApi = true` by default.
Switch it to `false` only after the backend contract suite in `TESTING.md` passes.

## 6. Postman

Import `LibraryManagementAPI.postman_collection.json` and
`LibraryManagementAPI.postman_environment.json` from this folder,
activate the environment, then follow `TESTING.md`.

## Troubleshooting

| Symptom | Cause / Fix |
|---|---|
| 404 on first call | Missing `/api/v1` prefix or wrong port |
| Email endpoint 200 but no mail | SMTP failure is logged (Serilog), responses stay generic by contract |
| Postman SSL error | Dev-only: Settings → SSL certificate verification → off |
| PUT book → 400 rowVersion | Client must echo `rowVersion` from the last GET |
| PUT book → 409 | Stale `rowVersion` (someone updated first) — reload and retry |
