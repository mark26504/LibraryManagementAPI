namespace LibraryManagement.Services.Abstraction.Contracts.Authors
{
    public interface IAuthorService
    {
        Task<Result<IEnumerable<AuthorResponse>>> GetAllAuthorsAsync();
        Task<Result<AuthorResponse>> GetAuthorByIdAsync(Guid authorId);
        Task<Result<AuthorResponse>> CreateAuthorAsync(CreateAuthorRequest request);
        Task<Result> UpdateAuthorAsync(Guid authorId, UpdateAuthorRequest request);
        Task<Result> DeleteAuthorAsync(Guid authorId);
    }
}
