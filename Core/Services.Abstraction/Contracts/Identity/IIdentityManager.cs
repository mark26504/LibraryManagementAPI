namespace LibraryManagement.Services.Abstraction.Contracts.Identity
{
    public interface IIdentityManager
    {
        Task<Result> CreateUserAsync(UserRegistrationDto registrationDto, string role);
        Task<Result<IdentityUserInfo>> ValidateCredentialsAsync(UserLoginDto loginDto);
        Task<Result<IdentityUserInfo>> GetUserByIdAsync(string userId);
    }
}
