# Testing Guide

## Postman Contract Suite

Import the collection + environment from `Docs/`, activate the environment,
then run folders **00 → 06** in order (manually or via Collection Runner).

### Auto-captured variables

| Request | Sets |
|---|---|
| Login Admin / Librarian / Member / Member2 | adminToken / librarianToken / memberToken / member2Token |
| GET /users | userId (first Member without Admin role) |
| GET /authors · GET /categories | authorId · categoryId |
| GET /books | bookId, bookRowVersion, bookTitle, bookIsbn, bookDescription, bookPublicationDate, bookCategoryId |
| POST /borrowings | borrowingId |
| Confirm/Reset Password (pre-request) | emailToken, emailLinkEmail — parsed from `emailLinkUrl` pasted from the received email |

### Per-folder expectations

| Folder | Key expectations |
|---|---|
| 00 Health | 200 Healthy, anonymous |
| 01 Auth | login 200 + refresh cookie; register 204; refresh 200 with cookie rotation; revoke 200 with cookie cleared; confirm/reset 200 with valid email token, 400 otherwise |
| 02 Users | paged 200; pageSize capped at 50; pageNumber=0 → 400; status/roles 200; self-role change → 403; last active admin → 409 |
| 03 Catalog | CRUD 201/204; duplicate category name → 409; delete with history → archived (isActive=false), 204 |
| 04 Books | full contract shape incl. category/authors/rowVersion; filters (isbn, search, isAvailable, isActive, sortBy/sortDirection); create 201 with availableCopies = totalCopies; update 204 / 400 missing rowVersion / 409 stale; cover upload 200 `{url}`, `.exe` or >5MB → 400 |
| 05 Borrowings | member borrow 200/201; duplicate / unavailable / limit → 409; cross-user `/my/{id}` → 404; return 200; second return → 409; staff list & overdue 200 |
| 06 Cross-cutting | 401 without token; 403 wrong role; 404 unknown guid |

### Expected behaviors (not failures)

1. **Register re-run** → 409 duplicate email.
2. **Upload Cover** stores a machine-local file path; replace the file source when running elsewhere.
3. **Roles** elevates the captured member to Admin; on a third runner pass the Users capture
   finds no Member-only user. Add a "Restore Roles" request (`{"roles":["Member"]}`) for endless re-runs.
4. **User By Id** uses a fixed guid for manual inspection.
5. **429 rate limit** is exercised manually: fire 11 consecutive logins; the 11th returns 429 + `Retry-After`.

## Manual Contract Scenarios (gate before release)

### Book rowVersion matrix
1. GET book → copy `rowVersion`
2. PUT with it → **204**
3. PUT with the old value → **409** `Book.ConcurrencyConflict`
4. PUT without the field (valid JSON) → **400** rowVersion required

### Final-copy race
1. Set a book to `totalCopies=1`, `availableCopies=1`
2. Two different members borrow it simultaneously
3. Exactly one succeeds; the other gets **409**; `availableCopies` ends at 0 (never negative);
   staff list shows a single active record

### Refresh rotation & revocation
1. login → refresh → cookie value changes; replaying the old cookie → **401**
2. revoke → refresh → **401**
3. reset password → all previous refresh cookies → **401**

### Email tokens are single-use
Confirming (or resetting) twice with the same token → second call **400**

### Administrative guards
- Changing your own roles → **403**
- Disabling the last active admin / removing Admin from them → **409**

### Inventory invariant
- PUT `totalCopies` below currently borrowed copies → **409** `Book.InventoryConflict`
- POST with `totalCopies` < 1 → **400**

## Error Contract Spot Checks

Every error response must contain `type, title, status, detail, instance, traceId`
(plus a camelCase `errors` dictionary on validation failures) and never a stack trace.
