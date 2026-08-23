namespace LibraryManagement.Persistence.Repositories
{
    internal sealed class BookRepository : GenericRepository<Book>, IBookRepository
    {
        public BookRepository(ApplicationDbContext dbContext) : base(dbContext) { }

        public async Task<IEnumerable<Book>> GetAllBooksAsync()
            => await FindAll(false)
            .ToListAsync();

        public async Task<Book?> GetBookByIdAsync(Guid id, bool trackChanges)
            => await FindByCondition(b => b.Id == id, trackChanges)
                .Include(b => b.BookAuthors)
                .FirstOrDefaultAsync();

        public async Task<bool> HasBorrowingRelationsAsync(Guid id)
            => await FindByCondition(b => b.Id == id, false)
                .AnyAsync(b => b.BorrowingRecords.Any());
    }
}
