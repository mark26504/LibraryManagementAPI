# Entity Relationship Diagram

The initial ERD will include the following main entities:

- ApplicationUser
- Book
- Author
- Category
- BookAuthor
- BorrowingRecord
- RefreshToken

## Initial Relationships

- Category has many Books.
- Book has many Authors.
- Author has many Books.
- BookAuthor represents the many-to-many relationship between Books and Authors.
- ApplicationUser has many BorrowingRecords.
- Book has many BorrowingRecords.
- ApplicationUser has RefreshTokens.

The ERD will be finalized after the Domain entities and EF Core relationships are configured.