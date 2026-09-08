using LibraryManagement.Domain.Contracts.BorrowingParamters;
using LibraryManagement.Domain.Enums;

namespace LibraryManagement.Services.Implementations.BorrowingRecords
{
    internal sealed class BorrowingService : IBorrowingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public BorrowingService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // Member / Self-Service Operations
        public async Task<Result<BorrowingResponse>> BorrowBookAsync(string userId, CreateBorrowingRequest request)
        {
            var book = await _unitOfWork.Books.GetBookByIdAsync(request.BookId, true);

            // Validation
            if (book == null || !book.IsActive)
                return Result<BorrowingResponse>.Failure
                    (new Error("Book.NotFound", "The requested book does not exist or is not available for borrowing."));
            
            if (book.AvailableCopies <= 0)
                return Result<BorrowingResponse>.Failure
                    (new Error("Book.NotAvailable", "The requested book is not available for borrowing."));
            
            // Bussiness Rule
            var userBorrowings = await _unitOfWork.Borrowings.CountActiveBorrowingsAsync(userId, false);
            if (userBorrowings >= 5)
                return Result<BorrowingResponse>.Failure
                    (new Error("Borrowing.LimitExceeded", "You have reached the maximum limit of 5 active borrowings."));

            var duplicatedBorrowing = await _unitOfWork.Borrowings.HasActiveBorrowingForBookAsync(userId, book.Id);
            if (duplicatedBorrowing)
                return Result<BorrowingResponse>.Failure
                    (new Error("Borrowing.Duplicate", "You have already borrowed this book."));

            // Create Borrowing Record
            var borrowing = new BorrowingRecord
            {
                UserId = userId,
                BookId = request.BookId,
                BorrowedAt = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(14), // 2 weeks borrowing period
            };

            book.AvailableCopies--;
            book.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.Borrowings.Create(borrowing);

            try
            {
               await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception ex) when (ex.GetType().Name == "DbUpdateConcurrencyException")
            {
                return Result<BorrowingResponse>.Failure(
                    new Error("Book.ConcurrencyConflict", "The book was borrowed by another user. Please try again."));
            }
            return Result<BorrowingResponse>.Success(_mapper.Map<BorrowingResponse>(borrowing));
        }

        public async Task<Result<BorrowingResponse>> ReturnBookAsync(Guid id, string userId, bool isStaff)
        {
            var borrowing = await _unitOfWork.Borrowings.GetBorrowingByIdAsync(id, true);
            if (borrowing == null)
                return Result<BorrowingResponse>.Failure(
                    new Error("Borrowing.NotFound", "The requested borrowing does not exist."));
            
            if(!isStaff && userId != borrowing.UserId)
                return Result<BorrowingResponse>.Failure(
                    new Error("Borrowing.Unauthorized", "You cannot return a book borrowed by another member."));

            if (borrowing.ReturnedAt != null)
                return Result<BorrowingResponse>.Failure(
                    new Error("Borrowing.AlreadyReturned", "This book has already been returned."));

            // Update the borrowing record
            borrowing.ReturnedAt = DateTime.UtcNow;
            borrowing.Status = BorrowingStatus.Returned;
            borrowing.Book.AvailableCopies++;
            borrowing.Book.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception ex) when (ex.GetType().Name == "DbUpdateConcurrencyException")
            {
                return Result<BorrowingResponse>.Failure(
                    new Error("Borrowing.ConcurrencyConflict", "The borrowing record was modified by another user. Please try again."));
            }

            return Result<BorrowingResponse>.Success(_mapper.Map<BorrowingResponse>(borrowing));
        }

        public async Task<Result<PagedResponse<BorrowingResponse>>> GetMyBorrowingsAsync(string userId, BorrowingParameters parameters)
        {
            // 1. Enforce member isolation
            parameters.UserId = userId;

            // 2. Map to Domain
            var queryParams = MapToDomainParameters(parameters);

            // 3. Execute
            var (items, totalCount) = await _unitOfWork.Borrowings.GetBorrowingsAsync(queryParams, false);

            // 4. Map & Return
            var mappedItems = _mapper.Map<IEnumerable<BorrowingResponse>>(items);
            var pagedResponse = new PagedResponse<BorrowingResponse>(mappedItems, totalCount, parameters.PageNumber, parameters.PageSize);

            return Result<PagedResponse<BorrowingResponse>>.Success(pagedResponse);
        }

        public async Task<Result<BorrowingResponse>> GetMyBorrowingByIdAsync(Guid id, string userId)
        {
            var borrowing = await _unitOfWork.Borrowings.GetBorrowingByIdAsync(id, false);

            if (borrowing == null || borrowing.UserId != userId)
            {
                return Result<BorrowingResponse>.Failure(
                    new Error("Borrowing.NotFound", "The requested borrowing does not exist."));
            }

            return Result<BorrowingResponse>.Success(_mapper.Map<BorrowingResponse>(borrowing));
        }
        

        // Staff Operations
        public async Task<Result<PagedResponse<BorrowingResponse>>> GetAllBorrowingsAsync(BorrowingParameters parameters)
        {
            var queryParams = MapToDomainParameters(parameters);
            var (items, totalCount) = await _unitOfWork.Borrowings.GetBorrowingsAsync(queryParams, false);

            var mappedItems = _mapper.Map<IEnumerable<BorrowingResponse>>(items);
            var pagedResponse = new PagedResponse<BorrowingResponse>(mappedItems, totalCount, parameters.PageNumber, parameters.PageSize);

            return Result<PagedResponse<BorrowingResponse>>.Success(pagedResponse);
        }

        public async Task<Result<BorrowingResponse>> GetBorrowingByIdAsync(Guid id)
        {
            var borrowing = await _unitOfWork.Borrowings.GetBorrowingByIdAsync(id, false);

            if (borrowing == null)
            {
                return Result<BorrowingResponse>.Failure(
                    new Error("Borrowing.NotFound", "The requested borrowing does not exist."));
            }

            return Result<BorrowingResponse>.Success(_mapper.Map<BorrowingResponse>(borrowing));
        }

        public async Task<Result<PagedResponse<BorrowingResponse>>> GetOverdueBorrowingsAsync(BorrowingParameters parameters)
        {
            parameters.IsOverdue = true;
            parameters.Status = "active";

            var queryParams = MapToDomainParameters(parameters);
            var (items, totalCount) = await _unitOfWork.Borrowings.GetBorrowingsAsync(queryParams, false);

            var mappedItems = _mapper.Map<IEnumerable<BorrowingResponse>>(items);
            var pagedResponse = new PagedResponse<BorrowingResponse>(mappedItems, totalCount, parameters.PageNumber, parameters.PageSize);

            return Result<PagedResponse<BorrowingResponse>>.Success(pagedResponse);
        }

        #region Helper Method

        private BorrowingQueryParameters MapToDomainParameters(BorrowingParameters p)
        {
            return new BorrowingQueryParameters(
                p.SearchTerm, p.Status, p.UserId, p.BookId,
                p.BorrowedFrom, p.BorrowedTo, p.DueFrom, p.DueTo,
                p.IsOverdue, p.OrderBy, p.PageNumber, p.PageSize
            );
        }

        #endregion
    }
}
