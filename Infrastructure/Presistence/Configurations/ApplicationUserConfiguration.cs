namespace LibraryManagement.Persistence.Configurations
{
    public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            // String Constarints
            builder.Property(u => u.FirstName)
                .IsRequired()
                .HasMaxLength(60);
            builder.Property(u => u.LastName)
                .IsRequired()
                .HasMaxLength(60);

            // Relationships
            // One-to-Many ApplicationUser -> BorrowingRecords
            builder.HasMany(u => u.BorrowingRecords)
                .WithOne()
                .HasForeignKey(br => br.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // One-to-Many ApplicationUser -> RefreshTokens
            builder.HasMany(u => u.RefreshTokens)
                .WithOne(rt => rt.ApplicationUser)
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
