namespace LibraryManagement.Services.Abstraction.Contracts.Categoties
{
    public interface ICategoryService
    {
        Task<Result<IEnumerable<CategoryResponse>>> GetAllCategoriesAsync();
        Task<Result<CategoryResponse>> GetCategoryByIdAsync(Guid id);
        Task<Result<CategoryResponse>> CreateCategoryAsync(CreateCategoryRequest request);
        Task<Result> UpdateCategoryAsync(Guid id, UpdateCategoryRequest request);
        Task<Result> DeleteCategoryAsync(Guid id);
    }
}
