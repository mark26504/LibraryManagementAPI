namespace LibraryManagement.Persistence.Repositories
{
    internal sealed class BorrowingRepository : GenericRepository<BorrowingRecord>, IBorrowingRepository
    {
        public BorrowingRepository(ApplicationDbContext dbContext) : base(dbContext){ }

        public async Task<int> CountActiveBorrowingsAsync(string userId, bool trackChanges)
            => await FindByCondition(br => br.UserId == userId && br.ReturnedAt == null, trackChanges).CountAsync();
        

        public async Task<BorrowingRecord?> GetBorrowingByIdAsync(Guid id, bool trackChanges)
            => await FindByCondition(br => br.Id == id, trackChanges)
                    .Include(br => br.Book)
                    .FirstOrDefaultAsync();

        public async Task<(IEnumerable<BorrowingRecord> Items, int TotalCount)> GetBorrowingsAsync
            (BorrowingQueryParameters parameters, bool trackChanges)
        {
            var query = FindAll(trackChanges)
                .Include(br => br.Book)
                .AsQueryable();
            if (!string.IsNullOrWhiteSpace(parameters.UserId))
                query = query.Where(br => br.UserId == parameters.UserId);

            if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
                query = query.Where(br => br.Book.Title.Contains(parameters.SearchTerm));
            

            if (!string.IsNullOrWhiteSpace(parameters.Status))
            {
                if (parameters.Status.Equals("active", StringComparison.OrdinalIgnoreCase))
                    query = query.Where(br => br.ReturnedAt == null);
                else if (parameters.Status.Equals("returned", StringComparison.OrdinalIgnoreCase))
                    query = query.Where(br => br.ReturnedAt != null);
            }

            if (parameters.BookId.HasValue)
                query = query.Where(br => br.BookId == parameters.BookId.Value);
            if (parameters.BorrowedFrom.HasValue)
                query = query.Where(br => br.BorrowedAt >= parameters.BorrowedFrom.Value);
            if (parameters.BorrowedTo.HasValue)
                query = query.Where(br => br.BorrowedAt <= parameters.BorrowedTo.Value);
            if (parameters.DueFrom.HasValue)
                query = query.Where(br => br.DueDate >= parameters.DueFrom.Value);
            if (parameters.DueTo.HasValue)
                query = query.Where(br => br.DueDate <= parameters.DueTo.Value);
            if (parameters.IsOverdue.HasValue && parameters.IsOverdue.Value)
                query = query.Where(br => br.DueDate < DateTime.UtcNow && br.ReturnedAt == null);

            // Apply ordering
            var descending = string.Equals(
                parameters.SortDirection, "desc", StringComparison.OrdinalIgnoreCase);

            query = (parameters.SortBy ?? "borrowedat").ToLower() switch
            {
                "duedate" => descending
                    ? query.OrderByDescending(br => br.DueDate)
                    : query.OrderBy(br => br.DueDate),

                "status" => descending
                    ? query.OrderByDescending(br => br.Status)
                    : query.OrderBy(br => br.Status),

                _ => descending
                    ? query.OrderByDescending(br => br.BorrowedAt)
                    : query.OrderBy(br => br.BorrowedAt),
            };

            var totalCount = await query.CountAsync();
            
            var items = await query
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<bool> HasActiveBorrowingForBookAsync(string userId, Guid bookId)
            => await FindByCondition(br => br.UserId == userId &&
                                         br.BookId == bookId &&
                                         br.ReturnedAt == null, false).AnyAsync();
    }
}
