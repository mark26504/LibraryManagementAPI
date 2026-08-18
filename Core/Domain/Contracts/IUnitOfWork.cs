namespace LibraryManagement.Domain.Contracts
{
    public interface IUnitOfWork
    {
        IBookRepository Books { get; }
        IAuthorRepository Authors { get; }
        ICategoryRepository Categories { get; }
        IBorrowingRepository Borrowings { get; }
        Task<int> SaveChangesAsync();
    }
}
