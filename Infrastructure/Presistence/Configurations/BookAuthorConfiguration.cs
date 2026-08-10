namespace LibraryManagement.Persistence.Configurations
{
    public class BookAuthorConfiguration : IEntityTypeConfiguration<BookAuthor>
    {
        public void Configure(EntityTypeBuilder<BookAuthor> builder)
        {
            // Composite Primary Key
            builder.HasKey(ba => new { ba.BookId, ba.AuthorId });

            // Relationships
            // Many-to-One: BookAuthor -> Book
            builder.HasOne(ba => ba.Book)
                   .WithMany(b => b.BookAuthors)
                   .HasForeignKey(ba => ba.BookId)
                   .OnDelete(DeleteBehavior.Cascade);
            // Many-to-One: BookAuthor -> Author
            builder.HasOne(ba => ba.Author)
                   .WithMany(a => a.BookAuthors)
                   .HasForeignKey(ba => ba.AuthorId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
