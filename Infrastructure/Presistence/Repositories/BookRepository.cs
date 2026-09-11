namespace LibraryManagement.Persistence.Repositories
{
    internal sealed class BookRepository : GenericRepository<Book>, IBookRepository
    {
        public BookRepository(ApplicationDbContext dbContext) : base(dbContext) { }

        public async Task<(IEnumerable<Book> Books, int TotalCount)> GetAllBooksAsync(
                                                                        BookQueryParameters parameters,
                                                                        bool trackChanges)
        {
            var query = FindAll(trackChanges);

            if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
            {
                var search = parameters.SearchTerm.Trim();

                query = query.Where(b =>
                    b.Title.Contains(search) ||
                    b.ISBN.Contains(search) ||
                    b.BookAuthors.Any(ba => ba.Author.Name.Contains(search)));
            }

            if (!string.IsNullOrWhiteSpace(parameters.Isbn))
                query = query.Where(b => b.ISBN == parameters.Isbn);

            if (parameters.CategoryId.HasValue)
                query = query.Where(b => b.CategoryId == parameters.CategoryId);

            if (parameters.AuthorId.HasValue)
                query = query.Where(b => b.BookAuthors.Any(ba => ba.AuthorId == parameters.AuthorId));

            if (parameters.IsAvailable == true)
                query = query.Where(b => b.AvailableCopies > 0);
            else if (parameters.IsAvailable == false)
                query = query.Where(b => b.AvailableCopies <= 0);

            if (parameters.IsActive.HasValue)
                query = query.Where(b => b.IsActive == parameters.IsActive);

            var totalCount = await query.CountAsync();

            var descending = string.Equals(
                parameters.SortDirection, "desc", StringComparison.OrdinalIgnoreCase);

            query = (parameters.SortBy ?? "title").ToLower() switch
            {
                "publicationdate" => descending
                    ? query.OrderByDescending(b => b.PublicationDate)
                    : query.OrderBy(b => b.PublicationDate),

                "createdat" => descending
                    ? query.OrderByDescending(b => b.CreatedAt)
                    : query.OrderBy(b => b.CreatedAt),

                _ => descending
                    ? query.OrderByDescending(b => b.Title)
                    : query.OrderBy(b => b.Title),
            };

            var books = await query
                .Include(b => b.Category)
                .Include(b => b.BookAuthors)
                    .ThenInclude(ba => ba.Author)
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToListAsync();

            return (books, totalCount);
        }

        public async Task<Book?> GetBookByIdAsync(Guid id, bool trackChanges)
            => await FindByCondition(b => b.Id == id, trackChanges)
                .Include(b => b.BookAuthors)
                .FirstOrDefaultAsync();

        public async Task<bool> HasBorrowingRelationsAsync(Guid id)
            => await FindByCondition(b => b.Id == id, false)
                .AnyAsync(b => b.BorrowingRecords.Any());
    }
}
