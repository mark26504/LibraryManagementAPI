namespace LibraryManagement.Shared.Dtos.Books
{
    public record BookResponse
        (Guid Id, string Title, string ISBN, string Description, string? CoverImageUrl,
         Guid CategoryId, bool IsActive, DateTime CreatedAt, DateTime? UpdatedAt);
}
