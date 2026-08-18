namespace LibraryManagement.Persistence.Repositories
{
    internal sealed class AuthorRepository : GenericRepository<Author>, IAuthorRepository
    {

        public AuthorRepository(ApplicationDbContext dbContext) : base(dbContext) { }

        public async Task<IEnumerable<Author>> GetAllAuthorsAsync(bool trackChanges)
            => await FindAll(trackChanges).ToListAsync();

        public async Task<Author?> GetAuthorByIdAsync(Guid authorId, bool trackChanges)
            => await FindByCondition(author => author.Id == authorId, trackChanges).FirstOrDefaultAsync();

        public async Task<bool> HasBookRelationsAsync(Guid authorId)
            => await FindByCondition (author => 
                        author.Id == authorId, false)
            .AnyAsync(author => author.BookAuthors.Any());
    }
}
