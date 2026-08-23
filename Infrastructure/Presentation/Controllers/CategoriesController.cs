namespace LibraryManagement.Presentation.Controllers
{
    [ApiController]
    [Route("api/v1/categories")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCategoriesAsync()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();

            if (categories.IsFailure)
                return BadRequest(categories.Error);

            return Ok(categories.Value);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(Guid id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);

            if (category.IsFailure)
                return NotFound(category.Error);

            return Ok(category.Value);
        }

        [Authorize(Roles = "Admin, Librarian")]
        [HttpPost]
        public async Task<IActionResult> CreateCategoryAsync([FromBody] CreateCategoryRequest request)
        {
            var result = await _categoryService.CreateCategoryAsync(request);
            if (result.IsFailure)
                return BadRequest(result.Error);
            return CreatedAtAction(nameof(GetCategoryById), new { id = result.Value.Id }, result.Value);
        }

        [Authorize(Roles = "Admin, Librarian")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategoryAsync(Guid id, [FromBody] UpdateCategoryRequest request)
        {
            var result = await _categoryService.UpdateCategoryAsync(id, request);
            if (result.IsFailure)
                return NotFound(result.Error);
            return NoContent();
        }

        [Authorize(Roles = "Admin, Librarian")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategoryAsync(Guid id)
        {
            var result = await _categoryService.DeleteCategoryAsync(id);
            if (result.IsFailure)
                return NotFound(result.Error);
            return NoContent();
        }
    }
}
