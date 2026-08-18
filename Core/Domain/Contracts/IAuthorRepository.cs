namespace LibraryManagement.Domain.Contracts
{
    public interface IAuthorRepository : IGenericRepository<Author>
    {
        Task<IEnumerable<Author>> GetAllAuthorsAsync(bool trackChanges);
        Task<Author?> GetAuthorByIdAsync(Guid authorId, bool trackChanges);
        Task<bool> HasBookRelationsAsync(Guid authorId);
    }
}
