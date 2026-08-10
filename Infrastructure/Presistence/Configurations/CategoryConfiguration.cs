namespace LibraryManagement.Persistence.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            // Primary Key
            builder.HasKey(c => c.Id);
            // String Constraints
            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(100);
            builder.Property(c => c.Description)
                .HasMaxLength(500);

            // Unique Index
            builder.HasIndex(c => c.Name)
                .IsUnique();
        }
    }
}
