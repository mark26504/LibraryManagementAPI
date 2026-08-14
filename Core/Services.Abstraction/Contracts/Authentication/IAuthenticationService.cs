using LibraryManagement.Shared.Models.Authentication;

namespace LibraryManagement.Services.Abstraction.Contracts.Authentication
{
    public interface IAuthenticationService
    {
        Task<Result> RegisterUserAsync(UserRegistrationDto registrationDto);
        Task<Result<AuthenticationResult>> LoginAsync(UserLoginDto loginDto);
        Task<Result<AuthenticationResult>> RefreshTokenAsync(string refreshToken);
        Task<Result> RevokeTokenAsync(string refreshToken);
    }
}
