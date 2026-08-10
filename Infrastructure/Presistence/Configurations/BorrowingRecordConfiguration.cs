namespace LibraryManagement.Persistence.Configurations
{
    public class BorrowingRecordConfiguration : IEntityTypeConfiguration<BorrowingRecord>
    {
        public void Configure(EntityTypeBuilder<BorrowingRecord> builder)
        {
            // Primary key 
            builder.HasKey(br => br.Id);

            // String Constraints
            builder.Property(br => br.UserId)
                .IsRequired();

            // Relationships
            // One-to-Many Book -> Borrowing records
            builder.HasOne(br => br.Book)
                .WithMany(b => b.BorrowingRecords)
                .HasForeignKey(br => br.BookId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
