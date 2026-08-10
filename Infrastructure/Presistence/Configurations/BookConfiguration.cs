namespace LibraryManagement.Persistence.Configurations
{
    public class BookConfiguration : IEntityTypeConfiguration<Book>
    {
        public void Configure(EntityTypeBuilder<Book> builder)
        {
            // Primary Key
            builder.HasKey(b => b.Id);

            // String Constraints
            builder.Property(b => b.Title)
                .IsRequired()
                .HasMaxLength(240);
            builder.Property(b => b.Description)
                .IsRequired()
                .HasMaxLength(2000);
            builder.Property(b => b.ISBN)
                .IsRequired();

            // Unique Index
            builder.HasIndex(b => b.ISBN)
                .IsUnique();

            // RowVersion 
            builder.Property(b => b.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();

            // Relationships
            // One-to-Many with Category
            builder.HasOne(b => b.Category)
                .WithMany(c => c.Books)
                .HasForeignKey(b => b.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
