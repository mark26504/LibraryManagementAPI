using LibraryManagement.Shared.Models.Authentication;

namespace LibraryManagement.Services.Abstraction.Contracts.Authentication
{
    public interface IAuthenticationService
    {
        Task<Result> RegisterUserAsync(UserRegistrationDto registrationDto);
        Task<Result<AuthenticationResult>> LoginAsync(UserLoginDto loginDto);
        Task<Result<AuthenticationResult>> RefreshTokenAsync(string refreshToken);
        Task<Result> RevokeTokenAsync(string refreshToken);

        // Email Confirmation & Password Recovery
        Task<Result> SendEmailConfirmationAsync(string email);
        Task<Result> ConfirmEmailAsync(string email, string token);
        Task<Result> ForgotPasswordAsync(string email);
        Task<Result> ResetPasswordAsync(string email, string token, string newPassword);
    }
}
