namespace LibraryManagement.Services.Implementations.Books
{
    internal sealed class BookService : IBookService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IFileStorageService _storageService;

        public BookService(IUnitOfWork unitOfWork, IMapper mapper, IFileStorageService storageService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _storageService = storageService;
        }

        public async Task<Result<PagedResponse<BookResponse>>> GetAllBooksAsync(BookParameters parameters)
        {
            var queryParams = MapToDomainParameters(parameters);

            var books = await _unitOfWork.Books.GetAllBooksAsync(queryParams, false);

            var bookResponse = _mapper.Map<IEnumerable<BookResponse>>(books.Books);
            var pagedResponse = new PagedResponse<BookResponse>
            (
                bookResponse,
                books.TotalCount,
                parameters.PageNumber,
                parameters.PageSize
            );

            return Result<PagedResponse<BookResponse>>.Success(pagedResponse);
        }
        public async Task<Result<BookResponse>> GetBookByIdAsync(Guid id)
        {
            var book = await _unitOfWork.Books.GetBookByIdAsync(id, false);
            if (book == null)
                return Result<BookResponse>.Failure(
                    Error.NotFound("Book.NotFound", "Book not found."));

            var bookResponse = _mapper.Map<BookResponse>(book);
            return Result<BookResponse>.Success(bookResponse);
        }

        public async Task<Result<BookResponse>> CreateBookAsync(CreateBookRequest request)
        {
            if (request.AuthorIds == null || !request.AuthorIds.Any())
                return Result<BookResponse>.Failure(
                    Error.Validation("Book.AuthorsRequired", "At least one author is required."));

            if (request.TotalCopies < 1)
                return Result<BookResponse>.Failure(
                    Error.Validation("Book.InvalidCopies", "Total copies must be at least 1."));

            var categoryExists = await _unitOfWork.Categories.GetCategoryByIdAsync(request.CategoryId, false) is not null;
            if (!categoryExists)
                return Result<BookResponse>.Failure(
                    Error.NotFound("Category.NotFound", "The selected category does not exist."));

            foreach (var authorId in request.AuthorIds.Distinct())
            {
                var authorExists = await _unitOfWork.Authors.GetAuthorByIdAsync(authorId, false) is not null;
                if (!authorExists)
                    return Result<BookResponse>.Failure(
                        Error.NotFound("Author.NotFound", "One of the selected authors does not exist."));
            }

            var book = _mapper.Map<Book>(request);
            book.ISBN = NormalizeIsbn(book.ISBN);

            book.AvailableCopies = book.TotalCopies;

            foreach (var authorId in request.AuthorIds.Distinct())
            {
                book.BookAuthors.Add(new BookAuthor
                {
                    BookId = book.Id,
                    AuthorId = authorId
                });
            }

            _unitOfWork.Books.Create(book);

            try
            {
                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception ex) when (ex.GetType().Name == "DbUpdateException")
            {
                return Result<BookResponse>.Failure(
                    Error.Conflict("Book.DuplicateIsbn", "A book with this ISBN already exists."));
            }

            var createdBook = await _unitOfWork.Books.GetBookByIdAsync(book.Id, false);
            var bookResponse = _mapper.Map<BookResponse>(createdBook!);
            return Result<BookResponse>.Success(bookResponse);
        }
        public async Task<Result> UpdateBookAsync(Guid id, UpdateBookRequest request)
        {
            var book = await _unitOfWork.Books.GetBookByIdAsync(id, true);
            if (book is null)
                return Result.Failure(
                    Error.NotFound("Book.NotFound", "Book not found."));

            if (request.RowVersion is null || request.RowVersion.Length == 0)
                return Result.Failure(
                    Error.Validation("Book.RowVersionRequired", "rowVersion is required to update a book."));

            if (!book.RowVersion.SequenceEqual(request.RowVersion))
                return Result.Failure(
                    Error.Conflict("Book.ConcurrencyConflict", "The book was modified by another user. Please reload and try again."));

            if (request.AuthorIds is null || !request.AuthorIds.Any())
                return Result.Failure(
                    Error.Validation("Book.AuthorsRequired", "At least one author is required."));

            var categoryExists = await _unitOfWork.Categories.GetCategoryByIdAsync(request.CategoryId, false) is not null;
            if (!categoryExists)
                return Result.Failure(
                    Error.NotFound("Category.NotFound", "The selected category does not exist."));

            foreach (var authorId in request.AuthorIds.Distinct())
            {
                var authorExists = await _unitOfWork.Authors.GetAuthorByIdAsync(authorId, false) is not null;
                if (!authorExists)
                    return Result.Failure(
                        Error.NotFound("Author.NotFound", "One of the selected authors does not exist."));
            }

            var borrowedCopies = book.TotalCopies - book.AvailableCopies;

            if (request.TotalCopies < borrowedCopies)
                return Result.Failure(
                    Error.Conflict("Book.InventoryConflict", "Total copies cannot be lower than the number of currently borrowed copies."));

            _mapper.Map(request, book);
            book.ISBN = NormalizeIsbn(book.ISBN);
            book.AvailableCopies = book.TotalCopies - borrowedCopies;

            var requestedAuthorIds = request.AuthorIds
                                    .Distinct()
                                    .ToHashSet();

            var relationsToRemove = book.BookAuthors
                .Where(ba => !requestedAuthorIds.Contains(ba.AuthorId))
                .ToList();

            foreach (var relation in relationsToRemove)
            {
                book.BookAuthors.Remove(relation);
            }

            var existingAuthorIds = book.BookAuthors
                .Select(ba => ba.AuthorId)
                .ToHashSet();

            foreach (var authorId in requestedAuthorIds.Where(aid => !existingAuthorIds.Contains(aid)))
            {
                book.BookAuthors.Add(new BookAuthor
                {
                    BookId = book.Id,
                    AuthorId = authorId
                });
            }

            book.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception ex) when (ex.GetType().Name == "DbUpdateConcurrencyException")
            {
                return Result.Failure(
                    Error.Conflict("Book.ConcurrencyConflict", "The book was modified by another user. Please reload and try again."));
            }
            catch (Exception ex) when (ex.GetType().Name == "DbUpdateException")
            {
                return Result.Failure(
                    Error.Conflict("Book.DuplicateIsbn", "A book with this ISBN already exists."));
            }

            return Result.Success();
        }
        public async Task<Result> DeleteBookAsync(Guid id)
        {
            var book = await _unitOfWork.Books.GetBookByIdAsync(id, true);
            if (book is null)
                return Result.Failure(
                    Error.NotFound("Book.NotFound", "Book not found."));

            var bookRelationship = await _unitOfWork.Books.HasBorrowingRelationsAsync(id);
            if (bookRelationship)
            {
                book.IsActive = false;
                book.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                _unitOfWork.Books.Delete(book);
            }

            await _unitOfWork.SaveChangesAsync();
            return Result.Success();
        }

        public async Task<Result<string>> UploadBookCoverAsync(Guid bookId, Stream fileStream, string extention)
        {
            var book = await _unitOfWork.Books.GetBookByIdAsync(bookId, true);
            if (book is null)
                return Result<string>.Failure(
                    Error.NotFound("Book.NotFound", "Book not found."));

            if (!string.IsNullOrEmpty(book.CoverImageUrl))
                _storageService.DeleteFile(book.CoverImageUrl);

            var filePath = await _storageService.SaveFileAsync(fileStream, extention, "uploads/books");

            book.CoverImageUrl = filePath;
            book.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync();

            return Result<string>.Success(filePath);
        }

        #region Helper Methods

        // Canonicalize ISBN before uniqueness checks (contract section 10)
        private static string NormalizeIsbn(string isbn)
            => new string(isbn.Where(char.IsLetterOrDigit).ToArray()).ToUpperInvariant();

        private static BookQueryParameters MapToDomainParameters(BookParameters p)
            => new(
                p.PageNumber,
                p.PageSize,
                p.SearchTerm,
                string.IsNullOrWhiteSpace(p.Isbn) ? null : NormalizeIsbn(p.Isbn),
                p.CategoryId,
                p.AuthorId,
                p.IsAvailable,
                p.IsActive,
                p.SortBy,
                p.SortDirection);
        #endregion
    }
}