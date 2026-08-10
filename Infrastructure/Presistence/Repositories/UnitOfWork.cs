namespace LibraryManagement.Persistence.Repositories
{
    internal sealed class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _dbContext;

        public UnitOfWork(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        private BookRepository? _booksRepository;
        public IBookRepository BooksRepo => _booksRepository ??= new BookRepository(_dbContext);

        private AuthorRepository? _authorsRepository;
        public IAuthorRepository AuthorsRepo => _authorsRepository ??= new AuthorRepository(_dbContext);

        private CategoryRepository? _categoriesRepository;
        public ICategoryRepository CategoriesRepo => _categoriesRepository ??= new CategoryRepository(_dbContext);

        private BorrowingRepository? _borrowingRepository;
        public IBorrowingRepository BorrowingsRepo => _borrowingRepository ??= new BorrowingRepository(_dbContext);

        public async Task<int> SaveChangesAsync() => await _dbContext.SaveChangesAsync();
    }
}
