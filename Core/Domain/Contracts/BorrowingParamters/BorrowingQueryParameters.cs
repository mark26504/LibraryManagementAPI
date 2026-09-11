namespace LibraryManagement.Domain.Contracts.BorrowingParamters
{
    public record BorrowingQueryParameters(
            string? SearchTerm,
            string? Status,
            string? UserId,
            Guid? BookId,
            DateTime? BorrowedFrom,
            DateTime? BorrowedTo,
            DateTime? DueFrom,
            DateTime? DueTo,
            bool? IsOverdue,
            string? SortBy,
            string? SortDirection,
            int PageNumber,
            int PageSize
        );
}
