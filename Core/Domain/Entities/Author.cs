namespace LibraryManagement.Domain.Entities
{
    public class Author
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = null!;
        public string Biography { get; set; } = null!;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        #region Book (1)<->(M) BookAuthor (M)<->(1) Author

        // Nav Prop
        public ICollection<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>();
        #endregion

    }
}
