namespace LibraryManagement.Domain.Contracts
{
    public interface IBookRepository : IGenericRepository<Book>
    {
        Task<IEnumerable<Book>> GetAllBooksAsync();
        Task<Book?> GetBookByIdAsync(Guid id, bool trackChanges);
        Task<bool> HasBorrowingRelationsAsync(Guid id);
    }
}
