namespace LibraryManagement.Domain.Contracts.BookParameters
{
    public record BookQueryParameters(
        int PageNumber,
        int PageSize,
        string? SearchTerm,
        string? Isbn,
        Guid? CategoryId,
        Guid? AuthorId,
        bool? IsAvailable,
        bool? IsActive,
        string? SortBy,
        string? SortDirection);
}
