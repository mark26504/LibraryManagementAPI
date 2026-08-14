namespace LibraryManagement.Shared.Dtos.Authentication
{
    public record AuthenticationUserDto
        (string Id, string FirstName, string LastName, string Email, IEnumerable<string> Roles);
}
