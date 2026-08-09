namespace LibraryManagement.Domain.Entities
{
    public class BookAuthor
    {
        #region Book (1)<->(M) BookAuthor (M)<->(1) Author
        // Nav Props
        public Book Book { get; set; } = null!;
        public Author Author { get; set; } = null!;

        // FKs
        public Guid BookId { get; set; } = Guid.Empty;
        public Guid AuthorId { get; set; } = Guid.Empty;
        #endregion
    }
}
