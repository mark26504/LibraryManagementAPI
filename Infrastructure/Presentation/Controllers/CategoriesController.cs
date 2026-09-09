namespace LibraryManagement.Presentation.Controllers
{
    [ApiController]
    [Route("api/v1/categories")]
    public class CategoriesController : ApiControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // GET: api/v1/categories
        [HttpGet]
        public async Task<IActionResult> GetCategoriesAsync()
        {
            var result = await _categoryService.GetAllCategoriesAsync();
            return result.IsSuccess ? Ok(result.Value) : Failure(result);
        }

        // GET: api/v1/categories/{id}
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetCategoryById(Guid id)
        {
            var result = await _categoryService.GetCategoryByIdAsync(id);
            return result.IsSuccess ? Ok(result.Value) : Failure(result);
        }

        // POST: api/v1/categories
        [HttpPost]
        [Authorize(Roles = "Admin,Librarian")]
        public async Task<IActionResult> CreateCategoryAsync([FromBody] CreateCategoryRequest request)
        {
            var result = await _categoryService.CreateCategoryAsync(request);
            if (result.IsFailure)
                return Failure(result);

            return CreatedAtAction(nameof(GetCategoryById), new { id = result.Value.Id }, result.Value);
        }

        // PUT: api/v1/categories/{id}
        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin,Librarian")]
        public async Task<IActionResult> UpdateCategoryAsync(Guid id, [FromBody] UpdateCategoryRequest request)
        {
            var result = await _categoryService.UpdateCategoryAsync(id, request);
            return result.IsSuccess ? NoContent() : Failure(result);
        }

        // DELETE: api/v1/categories/{id}
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin,Librarian")]
        public async Task<IActionResult> DeleteCategoryAsync(Guid id)
        {
            var result = await _categoryService.DeleteCategoryAsync(id);
            return result.IsSuccess ? NoContent() : Failure(result);
        }
    }
}