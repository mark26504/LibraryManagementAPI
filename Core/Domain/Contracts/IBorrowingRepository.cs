using LibraryManagement.Domain.Contracts.BorrowingParamters;

namespace LibraryManagement.Domain.Contracts
{
    public interface IBorrowingRepository : IGenericRepository<BorrowingRecord>
    {
        Task<int> CountActiveBorrowingsAsync(string userId, bool trackChanges);
        Task<bool> HasActiveBorrowingForBookAsync(string userId, Guid bookId);
        Task<BorrowingRecord?> GetBorrowingByIdAsync(Guid id, bool trackChanges);
        
        Task<(IEnumerable<BorrowingRecord> Items, int TotalCount)> GetBorrowingsAsync
            (BorrowingQueryParameters parameters, bool trackChanges);
    }
}