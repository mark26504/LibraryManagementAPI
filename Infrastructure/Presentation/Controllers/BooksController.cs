namespace LibraryManagement.Presentation.Controllers
{
    [ApiController]
    [Route("api/v1/books")]
    public class BooksController : ApiControllerBase
    {
        private const long MaxCoverSizeBytes = 5 * 1024 * 1024;

        private static readonly string[] AllowedExtensions =
            { ".jpg", ".jpeg", ".png", ".webp" };

        private static readonly string[] AllowedContentTypes =
            { "image/jpeg", "image/png", "image/webp" };

        private readonly IBookService _bookService;

        public BooksController(IBookService bookService)
        {
            _bookService = bookService;
        }

        // GET: api/v1/books
        [HttpGet]
        public async Task<IActionResult> GetAllBooks([FromQuery] BookParameters parameters)
        {
            var result = await _bookService.GetAllBooksAsync(parameters);
            return result.IsSuccess ? Ok(result.Value) : Failure(result);
        }

        // GET: api/v1/books/{id}
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetBookById(Guid id)
        {
            var result = await _bookService.GetBookByIdAsync(id);
            return result.IsSuccess ? Ok(result.Value) : Failure(result);
        }

        // POST: api/v1/books
        [Authorize(Roles = "Admin,Librarian")]
        [HttpPost]
        public async Task<IActionResult> CreateBook([FromBody] CreateBookRequest request)
        {
            var result = await _bookService.CreateBookAsync(request);
            if (result.IsFailure)
                return Failure(result);

            return CreatedAtAction(nameof(GetBookById), new { id = result.Value.Id }, result.Value);
        }

        // PUT: api/v1/books/{id}
        [Authorize(Roles = "Admin,Librarian")]
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateBook(Guid id, [FromBody] UpdateBookRequest request)
        {
            var result = await _bookService.UpdateBookAsync(id, request);
            return result.IsSuccess ? NoContent() : Failure(result);
        }

        // DELETE: api/v1/books/{id}
        [Authorize(Roles = "Admin,Librarian")]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteBook(Guid id)
        {
            var result = await _bookService.DeleteBookAsync(id);
            return result.IsSuccess ? NoContent() : Failure(result);
        }

        // POST: api/v1/books/{id}/cover
        [Authorize(Roles = "Admin,Librarian")]
        [HttpPost("{id:guid}/cover")]
        public async Task<IActionResult> UploadCover(Guid id, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return Failure(Result.Failure(
                    Error.Validation("Book.CoverRequired", "No file uploaded.")));

            var extention = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!AllowedExtensions.Contains(extention))
                return Failure(Result.Failure(
                    Error.Validation("Book.CoverTypeNotAllowed", "Invalid file type. Only .jpg, .jpeg, .png, and .webp are allowed.")));

            if (!AllowedContentTypes.Contains(file.ContentType.ToLowerInvariant()))
                return Failure(Result.Failure(
                    Error.Validation("Book.CoverTypeNotAllowed", "Invalid file content type. Only image/jpeg, image/png, and image/webp are allowed.")));

            if (file.Length > MaxCoverSizeBytes)
                return Failure(Result.Failure(
                    Error.Validation("Book.CoverTooLarge", "File size exceeds the 5 MB limit.")));

            await using var stream = file.OpenReadStream();
            var result = await _bookService.UploadBookCoverAsync(id, stream, extention);

            if (result.IsFailure)
                return Failure(result);

            return Ok(new { url = result.Value });
        }
    }
}