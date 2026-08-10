namespace LibraryManagement.Persistence.Configurations
{
    public class AuthorConfiguration : IEntityTypeConfiguration<Author>
    {
        public void Configure(EntityTypeBuilder<Author> builder)
        {
            // Primary Key
            builder.HasKey(a => a.Id);

            // String Constraints
            builder.Property(a => a.Name)
                .IsRequired()
                .HasMaxLength(160);
            builder.Property(a => a.Biography)
                .HasMaxLength(1000);
        }
    }
}
