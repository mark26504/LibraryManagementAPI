namespace LibraryManagement.Shared.Dtos.Identity
{
    public record class IdentityUserInfo
        (string Id, string FirstName, string LastName, string Email, bool IsActive, IEnumerable<string> Roles);

}
