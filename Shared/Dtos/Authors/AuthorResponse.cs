namespace LibraryManagement.Shared.Dtos.Authors
{
    public record AuthorResponse
        (Guid Id, string Name, string Biography, bool IsActive, DateTime CreatedAt, DateTime UpdatedAt);
}
