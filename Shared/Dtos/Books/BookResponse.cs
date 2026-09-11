namespace LibraryManagement.Shared.Dtos.Books
{
    public record BookResponse(
        Guid Id,
        string Title,
        string ISBN,
        string Description,
        DateTime PublicationDate,
        int TotalCopies,
        int AvailableCopies,
        string? CoverImageUrl,
        CategoryResponse Category,
        IEnumerable<AuthorResponse> Authors,
        bool IsActive,
        DateTime CreatedAt,
        DateTime? UpdatedAt,
        byte[] RowVersion);
}
