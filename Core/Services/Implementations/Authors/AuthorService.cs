namespace LibraryManagement.Services.Implementations.Authors
{
    internal sealed class AuthorService : IAuthorService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AuthorService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<IEnumerable<AuthorResponse>>> GetAllAuthorsAsync()
        {
            var authors = await _unitOfWork.Authors.GetAllAuthorsAsync(false);

            var authorsResponse = _mapper.Map<IEnumerable<AuthorResponse>>(authors);

            return Result<IEnumerable<AuthorResponse>>.Success(authorsResponse);
        }

        public async Task<Result<AuthorResponse>> GetAuthorByIdAsync(Guid authorId)
        {
            var author = await _unitOfWork.Authors.GetAuthorByIdAsync(authorId, false);
            
            if (author == null)
                return Result<AuthorResponse>.Failure(
                    new Error("Author.NotFound", "Author not found."));
            
            var authorResponse = _mapper.Map<AuthorResponse>(author);

            return Result<AuthorResponse>.Success(authorResponse);
        }
        public async Task<Result<AuthorResponse>> CreateAuthorAsync(CreateAuthorRequest request)
        {
            var author = _mapper.Map<Author>(request);

            _unitOfWork.Authors.Create(author);
            await _unitOfWork.SaveChangesAsync();

            var mappedAuthor = _mapper.Map<AuthorResponse>(author);
            return Result<AuthorResponse>.Success(mappedAuthor);
        }


        public async Task<Result> UpdateAuthorAsync(Guid authorId, UpdateAuthorRequest request)
        {

            var author = await _unitOfWork.Authors
                .GetAuthorByIdAsync(authorId, true);

            if (author == null)
                return Result.Failure(new Error("Author.NotFound", "Author not found."));

            _mapper.Map(request, author);

            author.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync();

            return Result.Success();
        }
        public async Task<Result> DeleteAuthorAsync(Guid authorId)
        {
            var author = await _unitOfWork.Authors.GetAuthorByIdAsync(authorId, true);
            if (author == null)
                return Result.Failure(new Error("Author.NotFound", "Author not found."));

            var hasBookRelations = await _unitOfWork.Authors.HasBookRelationsAsync(authorId);

            if (hasBookRelations)
            {
                author.IsActive = false;
                author.UpdatedAt = DateTime.UtcNow;
            }
            else
                _unitOfWork.Authors.Delete(author);


            await _unitOfWork.SaveChangesAsync();
            return Result.Success();
        }
    }
}
