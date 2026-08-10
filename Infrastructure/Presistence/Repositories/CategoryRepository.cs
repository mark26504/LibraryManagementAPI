namespace LibraryManagement.Persistence.Repositories
{
    internal sealed class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {

        public CategoryRepository(ApplicationDbContext dbContext) : base(dbContext){ }
    }
}
