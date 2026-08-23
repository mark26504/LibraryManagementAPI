namespace LibraryManagement.Shared.Dtos.Books
{
    public record BookResponse
        (Guid Id, string Title, string ISBN, string Description,
         Guid CategoryId, bool IsActive, DateTime CreatedAt, DateTime? UpdatedAt);
}
