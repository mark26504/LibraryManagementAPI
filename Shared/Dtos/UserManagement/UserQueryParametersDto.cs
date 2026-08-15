namespace LibraryManagement.Shared.Dtos.UserManagement
{
    public record UserQueryParametersDto
        (int PageNumber = 1,
         int PageSize = 10,
         string? Search = null,
         string? Role = null,
         bool? IsActive = null,
         string? SortBy = null,
         string? SortDirection = null);
}
