namespace LibraryManagement.Shared.Dtos.UserManagement
{
    public record UserDto
        (string Id, string FirstName, string LastName, string Email, bool IsActive, IEnumerable<string> Roles);
}
