namespace LibraryManagement.Shared.Dtos.Books
{
    public class BookParameters
    {
        private const int MaxPageSize = 50;

        public int PageNumber { get; set; } = 1;

        private int _pageSize = 10;
        public int PageSize
        {
            get { return _pageSize; }
            set { _pageSize = value > MaxPageSize ? MaxPageSize : value; }
        }

        public string? SearchTerm { get; set; }
        public Guid? CategoryId { get; set; }
        public string? OrderBy { get; set; } = "title";

    }
}
