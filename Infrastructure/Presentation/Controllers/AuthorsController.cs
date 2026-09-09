namespace LibraryManagement.Presentation.Controllers
{
    [ApiController]
    [Route("api/v1/authors")]
    public class AuthorsController : ApiControllerBase
    {
        private readonly IAuthorService _authorService;

        public AuthorsController(IAuthorService authorService)
        {
            _authorService = authorService;
        }

        // GET: api/v1/authors
        [HttpGet]
        public async Task<IActionResult> GetAuthorsAsync()
        {
            var result = await _authorService.GetAllAuthorsAsync();
            return result.IsSuccess ? Ok(result.Value) : Failure(result);
        }

        // GET: api/v1/authors/{id}
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetAuthorById(Guid id)
        {
            var result = await _authorService.GetAuthorByIdAsync(id);
            return result.IsSuccess ? Ok(result.Value) : Failure(result);
        }

        // POST: api/v1/authors
        [HttpPost]
        [Authorize(Roles = "Admin,Librarian")]
        public async Task<IActionResult> CreateAuthorAsync([FromBody] CreateAuthorRequest request)
        {
            var result = await _authorService.CreateAuthorAsync(request);
            if (result.IsFailure)
                return Failure(result);

            return CreatedAtAction(nameof(GetAuthorById), new { id = result.Value.Id }, result.Value);
        }

        // PUT: api/v1/authors/{id}
        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin,Librarian")]
        public async Task<IActionResult> UpdateAuthorAsync(Guid id, [FromBody] UpdateAuthorRequest request)
        {
            var result = await _authorService.UpdateAuthorAsync(id, request);
            return result.IsSuccess ? NoContent() : Failure(result);
        }

        // DELETE: api/v1/authors/{id}
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin,Librarian")]
        public async Task<IActionResult> DeleteAuthorAsync(Guid id)
        {
            var result = await _authorService.DeleteAuthorAsync(id);
            return result.IsSuccess ? NoContent() : Failure(result);
        }
    }
}