namespace LibraryManagement.Services.Abstraction.Contracts.Identity
{
    public interface IIdentityManager
    {
        // Authentication
        Task<Result> CreateUserAsync(UserRegistrationDto registrationDto, string role);
        Task<Result<IdentityUserInfo>> ValidateCredentialsAsync(UserLoginDto loginDto);
        Task<Result<IdentityUserInfo>> GetUserByIdAsync(string userId);

        // User Management
        Task<Result<PagedResponse<IdentityUserInfo>>> GetUsersAsync(UserQueryParametersDto queryParameters);
        Task<Result> UpdateUserStatusAsync(string userId, bool isActive);
        Task<Result> UpdateUserRolesAsync(string userId, IEnumerable<string> roles);
        Task<Result> UpdateUserProfileAsync(string userId, string firstName, string lastName);

        // Email Confirmation & Password Recovery
        Task<Result<string>> FindUserIdByEmailAsync(string email);
        Task<Result<string>> GenerateEmailConfirmationTokenAsync(string userId);
        Task<Result> ConfirmEmailAsync(string userId, string token);
        Task<Result<string>> GeneratePasswordResetTokenAsync(string userId);
        Task<Result> ResetPasswordAsync(string userId, string token, string newPassword);
    }
}
