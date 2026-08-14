namespace LibraryManagement.Shared.Dtos.Authentication
{
    public record AuthenticationResponse(string AccessToken, DateTime AccessTokenExpiresAt, AuthenticationUserDto User);

}
