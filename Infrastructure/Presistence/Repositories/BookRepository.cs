namespace LibraryManagement.Persistence.Repositories
{
    internal sealed class BookRepository : GenericRepository<Book>, IBookRepository
    {
        public BookRepository(ApplicationDbContext dbContext) : base(dbContext) { }
    
    }
}
