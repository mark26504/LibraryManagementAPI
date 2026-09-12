# Business Rules (As-Built)

## Roles
Admin > Librarian > Member. Catalog reads public; catalog writes Librarian+;
borrowing self-service Member; borrowing staff operations Librarian+; users management Admin.

## Borrowing
- Max **5** active borrowings per member (`LibraryRules.MaxActiveBorrowings`).
- Due date = borrow time + **14 days** (`LibraryRules.StandardBorrowingPeriodDays`).
- One **active** borrowing per member per book; re-borrow allowed after return.
- Inactive books and zero-available books cannot be borrowed.
- Overdue = dueDate < now && returnedAt == null (computed at query time).

## Inventory Invariant
`availableCopies = totalCopies − activeBorrowedCopies` — server-owned, never client input.
- Create: `availableCopies = totalCopies`; `totalCopies < 1` → 400.
- Update: recomputed from new total; `totalCopies < borrowed` → 409 `Book.InventoryConflict`.
- Borrow: decrement; Return: increment; never negative (rowversion-protected).

## Concurrency
- `Book.rowVersion` (SQL Server rowversion) is the optimistic lock.
- Update requires client rowVersion: missing → 400; mismatch with DB → 409.
- True write races surface as `DbUpdateConcurrencyException` → 409 on books and borrowings.

## Archive vs Delete
Authors, categories and books **with history** are deactivated (`isActive=false`),
never hard-deleted; unreferenced records are removed. Archived items stay in historical
borrowing records and remain queryable via `isActive=false` filters.

## Uniqueness & Normalization
- ISBN stored canonical (digits/X only, uppercased); unique across books → 409 on duplicate.
- Author names and category names unique → 409 on duplicate.

## Pagination & Sorting
- Defaults: pageNumber 1, pageSize 10; pageSize capped at 50; invalid values → 400.
- Books sort: title (default), publicationDate, createdAt + asc/desc.
- Borrowings sort: borrowedAt (default desc), dueDate, status.
- Users sort: createdAt desc (default), firstName, lastName, email.

## Search Coverage
- Books: title, ISBN, author name. Users: first + last name.
- Authors: name. Categories: name. Borrowings: book title.

## Sessions & Email
- Refresh cookie rotated on every refresh; revoked on logout, password reset, disable.
- Email confirmation/reset tokens: single-use, delivered only via email, generic responses
  (no account enumeration).

## Administrative Guards
- Self role change → 403. Last active admin cannot be disabled or demoted → 409.
