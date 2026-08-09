namespace LibraryManagement.Domain.Entities
{
    public class Book
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; } = null!;
        public string ISBN { get; set; } = null!;
        public string Description { get; set; } = null!;
        public DateTime PublicationDate { get; set; }
        public int TotalCopies { get; set; }
        public int AvailableCopies { get; set; }
        public string? CoverImageUrl { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public byte[]? RowVersion { get; set; }

        #region Category -> Books [1:M]

        // Nav Prop
        public Category Category { get; set; } = null!;

        // FK
        public Guid CategoryId { get; set; }
        #endregion

        #region Book (1)<->(M) BookAuthor (M)<->(1) Author

        // Nav Prop
        public ICollection<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>();

        #endregion

        #region Book -> BorrowingRecord [1:M] 

        // Nav Prop
        public ICollection<BorrowingRecord> BorrowingRecords { get; set; } = new List<BorrowingRecord>();

        #endregion

    }
}
