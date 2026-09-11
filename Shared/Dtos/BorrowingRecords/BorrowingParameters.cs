namespace LibraryManagement.Shared.Dtos.BorrowingRecords
{
    public class BorrowingParameters
    {
        [Range(1, int.MaxValue, ErrorMessage = "Page number must be greater than zero.")]
        public int PageNumber { get; set; } = 1;

        private int _pageSize = 10;

        [Range(1, 50, ErrorMessage = "Page size must be between 1 and 50.")]
        public int PageSize
        {
            get { return _pageSize; }
            set { _pageSize = value > 50 ? 50 : value; }
        }

        public string? SearchTerm { get; set; }
        public string? Status { get; set; }
        public string? UserId { get; set; }
        public Guid? BookId { get; set; } 
        public DateTime? BorrowedFrom { get; set; }
        public DateTime? BorrowedTo { get; set; }
        public DateTime? DueFrom { get; set; }
        public DateTime? DueTo { get; set; }
        public bool? IsOverdue { get; set; }
        public string? SortBy { get; set; } = "borrowedAt";
        public string? SortDirection { get; set; } = "desc";
    }
}
