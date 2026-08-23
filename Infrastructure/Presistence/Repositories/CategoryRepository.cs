namespace LibraryManagement.Persistence.Repositories
{
    internal sealed class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {

        public CategoryRepository(ApplicationDbContext dbContext) : base(dbContext) { }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync(
            bool trackChanges)
            => await FindAll(trackChanges)
                .ToListAsync();

        public async Task<Category?> GetCategoryByIdAsync(
            Guid categoryId,
            bool trackChanges)
            => await FindByCondition(
                    category => category.Id == categoryId,
                    trackChanges)
                .FirstOrDefaultAsync();

        public async Task<bool> HasBookRelationsAsync(Guid categoryId)
            => await FindByCondition(
                    category => category.Id == categoryId,
                    false)
                .AnyAsync(category => category.Books.Any());
    }
}
