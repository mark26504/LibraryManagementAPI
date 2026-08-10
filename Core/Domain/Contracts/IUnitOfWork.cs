namespace LibraryManagement.Domain.Contracts
{
    public interface IUnitOfWork
    {
        IBookRepository BooksRepo { get; }
        IAuthorRepository AuthorsRepo { get; }
        ICategoryRepository CategoriesRepo { get; }
        IBorrowingRepository BorrowingsRepo { get; }
        Task<int> SaveChangesAsync();
    }
}
