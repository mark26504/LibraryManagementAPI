namespace LibraryManagement.Shared.Dtos.Categories
{
    public record CategoryResponse
        (Guid Id, string Name, string Description, bool IsActive, DateTime CreatedAt, DateTime? UpdatedAt);
}
