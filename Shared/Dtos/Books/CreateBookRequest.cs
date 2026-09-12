namespace LibraryManagement.Shared.Dtos.Books
{
    public record CreateBookRequest(
        string Title,
        string ISBN,
        string Description,
        DateTime PublicationDate,
        Guid CategoryId,
        IEnumerable<Guid> AuthorIds,
        int TotalCopies);
}
