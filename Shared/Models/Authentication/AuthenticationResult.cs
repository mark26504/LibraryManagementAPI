using LibraryManagement.Shared.Dtos.Authentication;

namespace LibraryManagement.Shared.Models.Authentication
{
    public record class AuthenticationResult(AuthenticationResponse Response, string RefreshToken);
}
