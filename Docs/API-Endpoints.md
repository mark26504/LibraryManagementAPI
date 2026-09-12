# API Endpoints (As-Built)

Base URL: `https://localhost:7113/api/v1` · All errors: RFC 9457 Problem Details with `traceId`.

## Auth (anonymous)

| Method & Route | Body | Success | Errors |
|---|---|---|---|
| POST /auth/register | firstName, lastName, email, password | 204 | 400 validation · 409 duplicate email |
| POST /auth/login | email, password | 200 `{accessToken, accessTokenExpiresAt, user}` + HttpOnly refresh cookie | 401 invalid · 403 disabled |
| POST /auth/refresh-token | `{}` + refresh cookie | 200 new access + rotated cookie | 401 invalid/revoked |
| POST /auth/revoke-token | `{}` + refresh cookie | 200 + cookie cleared | 401 |
| POST /auth/send-email-confirmation | email | 200 (generic) | 400 validation |
| POST /auth/confirm-email | email, token | 200 | 400 invalid/used token |
| POST /auth/forgot-password | email | 200 (generic) | 400 validation |
| POST /auth/reset-password | email, token, password | 200 + revokes all refresh tokens | 400 invalid token/weak password |

## Users (Admin only, except profile)

| Method & Route | Body / Query | Success | Errors |
|---|---|---|---|
| GET /users | pageNumber, pageSize, search, role, isActive, sortBy, sortDirection | 200 paged | 400 invalid pagination |
| GET /users/{id} | — | 200 | 404 |
| PATCH /users/{id}/status | `{isActive}` | 200 User | 404 · 409 last active admin |
| POST /users/{id}/roles | `{roles:[...]}` | 200 User | 400 empty/unknown role · 403 self · 404 · 409 last admin |
| PATCH /users/me/profile | firstName, lastName | 200 User | 400 · 404 (any authenticated role) |

## Authors & Categories (read: public · write: Librarian+)

| Method & Route | Success | Errors |
|---|---|---|
| GET /authors · /categories (paged, search, isActive, sortBy, sortDirection) | 200 | 400 |
| GET /authors/{id} · /categories/{id} | 200 | 404 |
| POST /authors · /categories | 201 | 400 · 409 duplicate name |
| PUT /authors/{id} · /categories/{id} | 204 | 400 · 404 · 409 duplicate |
| DELETE /authors/{id} · /categories/{id} | 204 (archived if referenced, removed otherwise) | 404 |

## Books (read: public · write: Librarian+)

| Method & Route | Body / Query | Success | Errors |
|---|---|---|---|
| GET /books | pageNumber, pageSize, search, isbn, categoryId, authorId, isAvailable, isActive, sortBy, sortDirection | 200 paged BookResponse | 400 |
| GET /books/{id} | — | 200 | 404 |
| POST /books | title, isbn, description, publicationDate, categoryId, authorIds[], totalCopies | 201 BookResponse | 400 authors/totalCopies · 404 category/author · 409 duplicate ISBN |
| PUT /books/{id} | same + totalCopies + **rowVersion** | 204 | 400 missing rowVersion/authors · 404 · 409 stale rowVersion / inventory / duplicate ISBN |
| DELETE /books/{id} | — | 204 (archived if borrowing history) | 404 |
| POST /books/{id}/cover | multipart `file` | 200 `{url}` | 400 ext/MIME/size · 404 |

## Borrowings

| Method & Route | Role | Success | Errors |
|---|---|---|---|
| POST /borrowings `{bookId}` | Member | 200 BorrowingResponse | 404 book · 409 NotAvailable / LimitExceeded / Duplicate |
| GET /borrowings/my (paged) | Member | 200 | 400 |
| GET /borrowings/my/{id} | Member | 200 | 404 (incl. not-owner) |
| POST /borrowings/{id}/return | Owner or Staff | 200 updated record | 404 · 403 other member's record · 409 AlreadyReturned |
| GET /borrowings (paged + filters) | Staff | 200 | 400 |
| GET /borrowings/{id} | Staff | 200 | 404 |
| GET /borrowings/overdue | Staff | 200 | 400 |

## Infrastructure

| Route | Auth | Result |
|---|---|---|
| GET /health | anonymous | 200 Healthy |
| Any protected route without/with wrong token | — | 401 / 403 |
| Rate-limited auth routes exceeded | — | 429 + `Retry-After` |
