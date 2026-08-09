# API Endpoints & Contract Conventions

## Global Conventions

- **Base Route:** All public API routes use the versioned prefix `/api/v1`.
- **JSON Casing:** All JSON sent to and from the client must use `camelCase`. C# properties remain `PascalCase`.
- **Identifiers:** All API-facing IDs (e.g., Guids) are serialized as `string`.
- **Dates & Times:** All dates and timestamps must use UTC and ISO-8601 format (e.g., `2026-08-08T12:00:00.000Z`).

## Authorization & Roles

- **Exact Roles:** `Admin`, `Librarian`, `Member`.
- **Registration:** Public registration always assigns the `Member` role.

## Error Handling

- **HTTP Errors:** All API errors will follow the standard `ProblemDetails` schema.

Standard fields:

- `type`
- `title`
- `status`
- `detail`
- `instance`
- `traceId`
- `errors` when validation errors exist

## Authentication Tokens

- **Access Token:** JWT returned in the response body.
- **Refresh Token:** The real refresh token is transported using a secure, `HttpOnly` cookie.
- Raw refresh tokens must **never** be exposed to frontend JavaScript.
- Raw refresh tokens must **never** be stored in the database.
- The database stores only a hash of the refresh token.
- Refresh tokens support rotation and revocation.

## Pagination Shape

Pagination responses use a body-based `PagedResult<T>` contract:

- `items`: T[]
- `pageNumber`: int (Default: 1)
- `pageSize`: int (Default: 10, Max: 50)
- `totalCount`: int
- `totalPages`: int
- `hasPreviousPage`: boolean
- `hasNextPage`: boolean