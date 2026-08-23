namespace LibraryManagement.Shared.Dtos.Books
{
    public record UpdateBookRequest
        (string Title, string ISBN, string Description, Guid CategoryId, IEnumerable<Guid> AuthorIds);
}
