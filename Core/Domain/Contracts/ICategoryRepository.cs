namespace LibraryManagement.Domain.Contracts
{
    public interface ICategoryRepository : IGenericRepository<Category>
    {
        Task<IEnumerable<Category>> GetAllCategoriesAsync(bool trackChanges);

        Task<Category?> GetCategoryByIdAsync(
            Guid categoryId,
            bool trackChanges);

        Task<bool> HasBookRelationsAsync(Guid categoryId);
    }
}
