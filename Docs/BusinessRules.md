# Business Rules

## Users

- Public registration always creates a Member.
- An inactive user cannot authenticate.
- Only Admin can manage user roles.

## Books

- A book must belong to a valid category.
- A book must have at least one author.
- ISBN must be unique.
- TotalCopies must be at least 1.
- AvailableCopies must remain between 0 and TotalCopies.

## Borrowing

- A Member can have a maximum of 5 active borrowings.
- Borrowing duration is 14 days.
- A Member cannot borrow the same book twice at the same time.
- Archived books cannot be borrowed.
- Books with no available copies cannot be borrowed.
- A borrowing can only be returned once.
- All borrowing dates use UTC.