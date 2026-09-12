# Architecture

## Layered Solution Map

```
LibraryManagement.API            Composition root: pipeline, DI, configuration, middleware
 └─> LibraryManagement.Presentation   Controllers (thin): routes, roles, binding → Problem Details
      └─> LibraryManagement.Services.Abstraction   Service + infrastructure contracts
           └─> LibraryManagement.Shared             DTOs, query params, PagedResponse<T>
      └─> LibraryManagement.Services   Business logic, Result mapping, AutoMapper profiles
           └─> LibraryManagement.Domain   Entities, enums, repository contracts, domain rules
 └─> LibraryManagement.Persistence   EF Core, Identity, repos, JWT/refresh store, MailKit, files
      └─> LibraryManagement.Services
```

| Project | May reference | Hosts |
|---|---|---|
| Domain | — | Entities, enums, `LibraryRules`, repository & UoW contracts, query records |
| Shared | — | Request/response DTOs, `BookParameters`, `PagedResponse<T>` |
| Services.Abstraction | Shared | `I*Service` contracts, `IFileStorageService`, `IEmailService`, token & identity contracts |
| Services | Domain, Services.Abstraction | Service implementations, validation, `Result` mapping, AutoMapper profiles |
| Persistence | Services | `ApplicationDbContext`, configurations, repositories, `IdentityManager`, `JwtTokenProvider`, refresh-token store, `MailKitEmailService`, `LocalFileStorageService`, migrations, DI registrations |
| Presentation | Services.Abstraction | Controllers inheriting `ApiControllerBase` |
| API | Persistence, Presentation | `Program.cs`, configuration extensions, `GlobalExceptionMiddleware` |

The `Services` project carries **no** `Microsoft.AspNetCore.App` framework reference.
Business services depend on `System.IO.Stream` abstractions, never on ASP.NET Core types
(`IWebHostEnvironment`, `IFormFile`, `HttpContext`). The compiler enforces this boundary.

## Request Lifecycle (example: POST /api/v1/borrowings)

1. Kestrel → rate-limit policy → CORS → routing (`/api/v1` prefix).
2. JWT bearer authentication + role authorization (`Member`).
3. Controller binds `CreateBorrowingRequest`, calls `IBorrowingService`.
4. Service loads the book tracked, validates availability / active-limit / duplicate,
   mutates inventory, creates the record, and calls `SaveChangesAsync` once
   (single implicit transaction).
5. Failures return `Result.Failure(Error)`; `ApiControllerBase` maps them to the
   frozen Problem Details shape. Success maps entity → DTO via AutoMapper.

## Core Patterns

- **Result / Result<T>**: every service outcome; controllers never throw for business rules.
- **Error codes**: stable strings (`Book.NotFound`, `Borrowing.LimitExceeded`, ...) consumed by clients.
- **PagedResponse<T>**: single pagination envelope (`items, totalCount, pageNumber, pageSize, totalPages, hasPreviousPage, hasNextPage`); pageSize capped at 50, invalid pagination → 400.
- **Unit of Work + Repository**: `FindByCondition(trackChanges)` controls tracking; includes live in repository queries.
- **Optimistic concurrency**: SQL Server `rowversion` on `Book`; update requires client rowVersion (400 missing / 409 stale via explicit comparison), races surface as `DbUpdateConcurrencyException` → 409.
- **Inventory invariant**: `availableCopies = totalCopies - activeBorrowed`; enforced on create, update, borrow, return; violation → 409 `Book.InventoryConflict`.
- **Soft archive**: authors/categories/books with history are deactivated (`isActive=false`), never hard-deleted.
- **Error contract**: RFC 9457 Problem Details with `traceId` on every path — Result failures, ModelState 400s (via `ApiBehaviorConfiguration`), 429, and unexpected 500 (`GlobalExceptionMiddleware`).

## Dependency Injection Map

| Contract | Implementation | Registered in |
|---|---|---|
| `IUnitOfWork` | `UnitOfWork` | Persistence |
| `I*Repository` | `*Repository` (internal) | Persistence |
| `IIdentityManager` | `IdentityManager` | Persistence |
| `ITokenProvider` | `JwtTokenProvider` | Persistence |
| `IRefreshTokenStore` | EF-backed store | Persistence |
| `IEmailService` | `MailKitEmailService` | Persistence |
| `IFileStorageService` | `LocalFileStorageService` | Persistence |
| `I*Service` (business) | `*Service` (internal) | Services |
