namespace LibraryManagement.Shared.Dtos.UserManagement
{
    public record UserQueryParametersDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "Page number must be greater than zero.")]
        public int PageNumber { get; init; } = 1;

        private int _pageSize = 10;

        [Range(1, 50, ErrorMessage = "Page size must be between 1 and 50.")]
        public int PageSize
        {
            get => _pageSize;
            init => _pageSize = value > 50 ? 50 : value;
        }

        public string? Search { get; init; }

        public string? Role { get; init; }

        public bool? IsActive { get; init; }

        public string? SortBy { get; init; } = "createdAt";

        public string? SortDirection { get; init; } = "desc";
    }
}
