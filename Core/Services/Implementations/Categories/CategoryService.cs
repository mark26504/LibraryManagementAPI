using LibraryManagement.Services.Abstraction.Contracts.Categoties;

namespace LibraryManagement.Services.Implementations.Categories
{
    internal sealed class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CategoryService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<Result<IEnumerable<CategoryResponse>>> GetAllCategoriesAsync()
        {
            var categories = await _unitOfWork.Categories.GetAllCategoriesAsync(false);

            var categoryResponses = _mapper.Map<IEnumerable<CategoryResponse>>(categories);

            return Result<IEnumerable<CategoryResponse>>.Success(categoryResponses);
        }

        public async Task<Result<CategoryResponse>> GetCategoryByIdAsync(Guid id)
        {
            var category = await _unitOfWork.Categories.GetCategoryByIdAsync(id, false);

            if (category is null)
                return Result<CategoryResponse>.Failure(
                    new Error("Category.NotFound", "Category not found."));

            var categoryResponse = _mapper.Map<CategoryResponse>(category);

            return Result<CategoryResponse>.Success(categoryResponse);
        }
        public async Task<Result<CategoryResponse>> CreateCategoryAsync(CreateCategoryRequest request)
        {
            var category = _mapper.Map<Category>(request);

            _unitOfWork.Categories.Create(category);
            await _unitOfWork.SaveChangesAsync();

            var categoryResponse = _mapper.Map<CategoryResponse>(category);

            return Result<CategoryResponse>.Success(categoryResponse);
        }

        public async Task<Result> DeleteCategoryAsync(Guid id)
        {
            var category = await _unitOfWork.Categories
                .GetCategoryByIdAsync(id, true);

            if (category is null)
                return Result.Failure(
                    new Error("Category.NotFound", "Category not found."));

            var hasBookRelations = await _unitOfWork.Categories
                .HasBookRelationsAsync(id);

            if (hasBookRelations)
            {
                category.IsActive = false;
                category.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                _unitOfWork.Categories.Delete(category);
            }

            await _unitOfWork.SaveChangesAsync();

            return Result.Success();
        }


        public async Task<Result> UpdateCategoryAsync(Guid id, UpdateCategoryRequest request)
        {
            var category = await _unitOfWork.Categories.GetCategoryByIdAsync(id, true);

            if (category is null)
                return Result.Failure(
                    new Error("Category.NotFound", "Category not found."));

            _mapper.Map(request, category);

            category.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync();

            return Result.Success();
        }
    }
}
