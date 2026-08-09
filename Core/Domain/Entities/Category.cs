namespace LibraryManagement.Domain.Entities
{
    public class Category
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        #region Category -> Books [1:M]

        // Nav Prop
        public ICollection<Book> Books { get; set; } = new List<Book>();

        #endregion
    }
}
