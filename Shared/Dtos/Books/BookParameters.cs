namespace LibraryManagement.Shared.Dtos.Books
{
    public class BookParameters
    {
        private const int MaxPageSize = 50;

        [Range(1, int.MaxValue, ErrorMessage = "Page number must be greater than zero.")]
        public int PageNumber { get; set; } = 1;

        private int _pageSize = 10;

        [Range(1, MaxPageSize, ErrorMessage = "Page size must be between 1 and 50.")]
        public int PageSize
        {
            get { return _pageSize; }
            set { _pageSize = value > MaxPageSize ? MaxPageSize : value; }
        }

        public string? SearchTerm { get; set; }
        public string? Isbn { get; set; }
        public Guid? CategoryId { get; set; }
        public Guid? AuthorId { get; set; }
        public bool? IsAvailable { get; set; }
        public bool? IsActive { get; set; }
        public string? SortBy { get; set; } = "title";
        public string? SortDirection { get; set; } = "asc";
    }
}
