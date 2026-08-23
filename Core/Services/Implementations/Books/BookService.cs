using LibraryManagement.Services.Abstraction.Contracts.Books;
using LibraryManagement.Shared.Dtos.Books;

namespace LibraryManagement.Services.Implementations.Books
{
    internal sealed class BookService : IBookService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public BookService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<IEnumerable<BookResponse>>> GetAllBooksAsync()
        {
            var books = await _unitOfWork.Books.GetAllBooksAsync();

            var bookResponses =
                _mapper.Map<IEnumerable<BookResponse>>(books);

            return Result<IEnumerable<BookResponse>>.Success(bookResponses);
        }

        public async Task<Result<BookResponse>> GetBookByIdAsync(Guid id)
        {
            var book = await _unitOfWork.Books.GetBookByIdAsync(id, false);
            if (book == null)
                return Result<BookResponse>.Failure(new Error("Book.NotFound", "Book not found."));

            var bookResponse = _mapper.Map<BookResponse>(book);
            return Result<BookResponse>.Success(bookResponse);
        }

        public async Task<Result<BookResponse>> CreateBookAsync(CreateBookRequest request)
        {
            var book = _mapper.Map<Book>(request);
            
            foreach (var authorId in request.AuthorIds)
            {
                book.BookAuthors.Add(new BookAuthor 
                {
                    BookId = book.Id,
                    AuthorId = authorId
                });
            }

            _unitOfWork.Books.Create(book);
            await _unitOfWork.SaveChangesAsync();

            var bookResponse = _mapper.Map<BookResponse>(book);
            return Result<BookResponse>.Success(bookResponse);
        }

        public async Task<Result> UpdateBookAsync(Guid id, UpdateBookRequest request)
        {
            var book = await _unitOfWork.Books.GetBookByIdAsync(id, true);

            if (book is null)
                return Result.Failure(new Error("Book.NotFound", "Book not found."));

            _mapper.Map(request, book);

            var requestedAuthorIds = request.AuthorIds
                                    .Distinct() 
                                    .ToHashSet();

            var relationtsToRemove = book.BookAuthors
                .Where(ba => !requestedAuthorIds.Contains(ba.AuthorId))
                .ToList();

            foreach (var relation in relationtsToRemove)
            {
                book.BookAuthors.Remove(relation);
            }

            var existingAuthorIds = book.BookAuthors
                .Select(ba => ba.AuthorId)
                .ToHashSet();

            var authorIdsToAdd = requestedAuthorIds
                    .Where(authorId => !existingAuthorIds.Contains(authorId));

            foreach (var authorId in authorIdsToAdd)
            {
                book.BookAuthors.Add(new BookAuthor
                {
                    BookId = book.Id,
                    AuthorId = authorId
                });
            }

            book.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<Result> DeleteBookAsync(Guid id)
        {
            var book = await _unitOfWork.Books.GetBookByIdAsync(id, true);

            if (book is null)
                return Result.Failure(
                    new Error("Book.NotFound", "Book not found."));

            var bookRelationship = await _unitOfWork.Books.HasBorrowingRelationsAsync(id);

            if (bookRelationship)
            {
                book.IsActive = false;
                book.UpdatedAt = DateTime.UtcNow;
            }
            else
                _unitOfWork.Books.Delete(book);
            await _unitOfWork.SaveChangesAsync();

            return Result.Success();
        }


    }
}
