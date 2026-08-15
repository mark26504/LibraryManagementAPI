using LibraryManagement.Shared.Dtos.UserManagement;

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
    }
}
