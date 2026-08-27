namespace LibraryManagement.Domain.Contracts
{
    public interface IBookRepository : IGenericRepository<Book>
    {
        Task<(IEnumerable<Book> Books, int TotalCount)> GetAllBooksAsync(int pageNumber,
                                                                         int pageSize,
                                                                         string? searchTerm,
                                                                         Guid? categoryId,
                                                                         string? orderBy,
                                                                         bool trackChanges);
        Task<Book?> GetBookByIdAsync(Guid id, bool trackChanges);
        Task<bool> HasBorrowingRelationsAsync(Guid id);
    }
}
