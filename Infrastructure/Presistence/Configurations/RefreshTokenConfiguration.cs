namespace LibraryManagement.Persistence.Configurations
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            // Primary Key
            builder.HasKey(rt => rt.Id);

            // String Constraints
            builder.Property(rt => rt.TokenHash)
                .IsRequired();
            builder.Property(rt => rt.UserId)
                .IsRequired();
        }
    }
}
