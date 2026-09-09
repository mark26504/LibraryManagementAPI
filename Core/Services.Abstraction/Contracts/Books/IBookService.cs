namespace LibraryManagement.Services.Abstraction.Contracts.Books
{
    public interface IBookService
    {
        Task<Result<PagedResponse<BookResponse>>> GetAllBooksAsync (BookParameters parameters);
        Task<Result<BookResponse>> GetBookByIdAsync(Guid id);
        Task<Result<BookResponse>> CreateBookAsync(CreateBookRequest request);
        Task<Result> UpdateBookAsync(Guid id, UpdateBookRequest request);
        Task<Result> DeleteBookAsync(Guid id);
        Task<Result<string>> UploadBookCoverAsync(Guid bookId, Stream fileStream, string extention);
    }
}
