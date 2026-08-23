namespace LibraryManagement.Presentation.Controllers
{
    [ApiController]
    [Route("api/v1/authors")]
    public class AuthorsController : ControllerBase
    {
        private readonly IAuthorService _authorService;

        public AuthorsController(IAuthorService authorService)
        {
            _authorService = authorService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAuthorsAsync()
        {
            var authors = await _authorService.GetAllAuthorsAsync();

            if (authors.IsFailure)
                return BadRequest(authors.Error);
            
            return Ok(authors.Value);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAuthorById(Guid id)
        {
            var author = await _authorService.GetAuthorByIdAsync(id);

            if (author.IsFailure)
                return NotFound(author.Error);

            return Ok(author.Value);
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Librarian")]
        public async Task<IActionResult> CreateAuthorAsync([FromBody] CreateAuthorRequest request)
        {
            var result = await _authorService.CreateAuthorAsync(request);

            if (result.IsFailure)
                return BadRequest(result.Error);

            return CreatedAtAction(nameof(GetAuthorById), new { id = result.Value.Id }, result.Value);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, Librarian")]
        public async Task<IActionResult> UpdateAuthorAsync(Guid id, [FromBody] UpdateAuthorRequest request)
        {
            var result = await _authorService.UpdateAuthorAsync(id, request);

            if (result.IsFailure)
            {
                if (result.Error.Code == "Author.NotFound")
                    return NotFound(result.Error);
                else
                    return BadRequest(result.Error);
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin, Librarian")]
        public async Task<IActionResult> DeleteAuthorAsync(Guid id)
        {
            var result = await _authorService.DeleteAuthorAsync(id);

            if (result.IsFailure)
            {
                if (result.Error.Code == "Author.NotFound")
                    return NotFound(result.Error);
                else
                    return BadRequest(result.Error);
            }

            return NoContent();
        }
    }
}
