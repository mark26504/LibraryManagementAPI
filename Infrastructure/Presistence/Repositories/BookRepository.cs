namespace LibraryManagement.Persistence.Repositories
{
    internal sealed class BookRepository : GenericRepository<Book>, IBookRepository
    {
        public BookRepository(ApplicationDbContext dbContext) : base(dbContext) { }

        public async Task<(IEnumerable<Book> Books, int TotalCount)> GetAllBooksAsync
            (int pageNumber, int pageSize, string? searchTerm, Guid? categoryId, string? orderBy, bool trackChanges)
        {
            var query = FindAll(trackChanges);

            if (!string.IsNullOrEmpty(searchTerm))
                query = query.Where(b => b.Title.Contains(searchTerm) || b.Description.Contains(searchTerm));

            if (categoryId.HasValue)
                query = query.Where(b => b.CategoryId == categoryId);

            var totalCount = await query.CountAsync();

            if (!string.IsNullOrEmpty(orderBy))
            {
                query = orderBy.ToLower() switch
                {
                    "title" => query.OrderBy(b => b.Title),
                    "title_desc" => query.OrderByDescending(b => b.Title),
                    "publicationdate" => query.OrderBy(b => b.PublicationDate),
                    "publicationdate_desc" => query.OrderByDescending(b => b.PublicationDate),
                    _ => query
                };
            }

            var books = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
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
