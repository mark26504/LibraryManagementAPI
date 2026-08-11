namespace LibraryManagement.Persistence.Configurations
{
    internal class RoleConfiguration : IEntityTypeConfiguration<IdentityRole>
    {
        public void Configure(EntityTypeBuilder<IdentityRole> builder)
        {
            builder.HasData
                (
                    new IdentityRole
                    {
                        Id = "1",
                        Name = "Admin",
                        NormalizedName = "ADMIN",
                        ConcurrencyStamp = "c96ba72d-5756-41b6-a902-bb055d8301cc"
                    },
                    new IdentityRole
                    {
                        Id = "2",
                        Name = "Librarian",
                        NormalizedName = "LIBRARIAN",
                        ConcurrencyStamp = "5ec66ade-44b2-4f8a-acad-e6cbf6ad1993"
                    },
                    new IdentityRole
                    {
                        Id = "3",
                        Name = "Member",
                        NormalizedName = "MEMBER",
                        ConcurrencyStamp = "97a222e2-ad95-4fb1-91ca-bac87446c0af"
                    }

                );
        }
    }
}
