namespace LibraryManagement.Shared.Dtos.BorrowingRecords
{
    public class BorrowingParameters
    {
        const int maxPageSize = 50;
        public int PageNumber { get; set; } = 1;

        private int _pageSize = 10;
        public int PageSize
        {
            get
            {
                return _pageSize;
            }
            set
            {
                _pageSize = (value > maxPageSize) ? maxPageSize : value;
            }
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
        public string? OrderBy { get; set; }

    }
}
