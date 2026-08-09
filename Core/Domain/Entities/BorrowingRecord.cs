namespace LibraryManagement.Domain.Entities
{
    public class BorrowingRecord
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime BorrowedAt { get; set; } = DateTime.UtcNow;
        public DateTime DueDate { get; set; } = DateTime.UtcNow.AddDays(14);
        public DateTime? ReturnedAt { get; set; }
        public BorrowingStatus Status { get; set; } = BorrowingStatus.Active;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        #region Book -> BorrowingRecord [1:M] 

        // Nav Prop
        public Book Book { get; set; } = null!;

        // Fk
        public Guid BookId { get; set; }
        #endregion

        #region ApplicationUser -> BorrowingRecord [1:M] 
        // Fk
        public string UserId { get; set; } = null!;

        #endregion

    }
}
