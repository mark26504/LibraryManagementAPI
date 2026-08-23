namespace LibraryManagement.Shared.Dtos.Books
{
    public record CreateBookRequest
        (string Title, string ISBN, string Description, Guid CategoryId, IEnumerable<Guid> AuthorIds);
}
