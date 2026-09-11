namespace LibraryManagement.Domain.Contracts
{
    public interface IBookRepository : IGenericRepository<Book>
    {
        Task<(IEnumerable<Book> Books, int TotalCount)> GetAllBooksAsync(
            BookQueryParameters parameters,
            bool trackChanges);

        Task<Book?> GetBookByIdAsync(Guid id, bool trackChanges);

        Task<bool> HasBorrowingRelationsAsync(Guid id);
    }
}
