namespace LibraryManagement.Persistence.Repositories
{
    internal sealed class AuthorRepository : GenericRepository<Author>, IAuthorRepository
    {
        public AuthorRepository(ApplicationDbContext dbContext) : base(dbContext) { }


    }
}
