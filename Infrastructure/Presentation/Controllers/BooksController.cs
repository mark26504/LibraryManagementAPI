using LibraryManagement.Domain.Entities;
using LibraryManagement.Services.Abstraction.Contracts.Books;
using LibraryManagement.Shared.Dtos.Books;

namespace LibraryManagement.Presentation.Controllers
{
    [ApiController]
    [Route("api/v1/books")]
    public class BooksController : ControllerBase
    {
        private readonly IBookService _bookService;

        public BooksController(IBookService bookService)
        {
            _bookService = bookService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllBooks()
        {
            var books = await _bookService.GetAllBooksAsync();

            if (books.IsFailure)
                return BadRequest(books.Error);

            return Ok(books.Value);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBookById(Guid id)
        {
            var book = await _bookService.GetBookByIdAsync(id);
            if (book.IsFailure)
                return NotFound(book.Error);
            return Ok(book.Value);
        }

        [Authorize(Roles = "Admin, Librarian")]
        [HttpPost]
        public async Task<IActionResult> CreateBook([FromBody] CreateBookRequest request)
        {
            var book = await _bookService.CreateBookAsync(request);
            if (book.IsFailure)
                return BadRequest(book.Error);
            return CreatedAtAction(nameof(GetBookById), new { id = book.Value.Id }, book.Value);
        }

        [Authorize(Roles = "Admin, Librarian")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBook(Guid id, [FromBody] UpdateBookRequest request)
        {
            var book = await _bookService.UpdateBookAsync(id, request);
            if (book.IsFailure)
            {
                if (book.Error.Code == "Book.NotFound")
                    return NotFound(book.Error);

                return BadRequest(book.Error);
            }
            return NoContent();
        }

        [Authorize(Roles = "Admin, Librarian")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(Guid id)
        {
            var result = await _bookService.DeleteBookAsync(id);
            if (result.IsFailure)
            {
                if (result.Error.Code == "Book.NotFound")
                    return NotFound(result.Error);

                return BadRequest(result.Error);
            }
            return NoContent();
        }
    }
}
