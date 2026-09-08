namespace LibraryManagement.Services.Abstraction.Contracts.BorrowingRecords
{
    public interface IBorrowingService
    {
        // Member / Self-Service Operations
        Task<Result<BorrowingResponse>> BorrowBookAsync(string userId, CreateBorrowingRequest request);
        Task<Result<BorrowingResponse>> ReturnBookAsync(Guid id, string userId, bool isStaff);
        Task<Result<PagedResponse<BorrowingResponse>>> GetMyBorrowingsAsync(string userId, BorrowingParameters parameters);
        Task<Result<BorrowingResponse>> GetMyBorrowingByIdAsync(Guid id, string userId);

        // Staff Operations
        Task<Result<PagedResponse<BorrowingResponse>>> GetAllBorrowingsAsync(BorrowingParameters parameters);
        Task<Result<BorrowingResponse>> GetBorrowingByIdAsync(Guid id);
        Task<Result<PagedResponse<BorrowingResponse>>> GetOverdueBorrowingsAsync(BorrowingParameters parameters);
    }
}
