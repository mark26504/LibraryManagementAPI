namespace LibraryManagement.Services.Abstraction.Contracts.Books
{
    public interface IBookService
    {
        Task<Result<IEnumerable<BookResponse>>> GetAllBooksAsync();
        Task<Result<BookResponse>> GetBookByIdAsync(Guid id);
        Task<Result<BookResponse>> CreateBookAsync(CreateBookRequest request);
        Task<Result> UpdateBookAsync(Guid id, UpdateBookRequest request);
        Task<Result> DeleteBookAsync(Guid id);
    }
}
