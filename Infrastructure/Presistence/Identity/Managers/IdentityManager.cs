using LibraryManagement.Services.Abstraction.Contracts.Identity;
using LibraryManagement.Shared.Dtos.Authentication;
using LibraryManagement.Shared.Dtos.Identity;
using LibraryManagement.Shared.Responses;

namespace LibraryManagement.Persistence.Identity.Managers
{
    internal sealed class IdentityManager : IIdentityManager
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public IdentityManager(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task<Result> CreateUserAsync(UserRegistrationDto registrationDto, string role)
        {
            var user = new ApplicationUser
            {
                UserName = registrationDto.Email,
                Email = registrationDto.Email,
                FirstName = registrationDto.FirstName,
                LastName = registrationDto.LastName
            };

            var result = await _userManager.CreateAsync(user, registrationDto.Password);
            if (!result.Succeeded)
            {
                var identityErrors  = result.Errors.First();

                var error = new Error(identityErrors.Code, identityErrors.Description);
                return Result.Failure(error);
            }
            await _userManager.AddToRoleAsync(user, role);
            return Result.Success();
        }

        public async Task<Result<IdentityUserInfo>> GetUserByIdAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Result<IdentityUserInfo>.Failure(new Error("User.NotFound", "User not found."));
            }

            var roles = await _userManager.GetRolesAsync(user);
            var identityUserInfo = new IdentityUserInfo
                (user.Id,
                user.FirstName,
                user.LastName,
                user.Email!,
                user.IsActive,
                roles);
            return Result<IdentityUserInfo>.Success(identityUserInfo);
        }

        public async Task<Result<IdentityUserInfo>> ValidateCredentialsAsync(UserLoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
            {
                var error = new Error("Authentication.InvalidCredentials", "Invalid email or password.");
                return Result<IdentityUserInfo>.Failure(error);
            }

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            if (!isPasswordValid)
            {
                var error = new Error("Authentication.InvalidCredentials", "Invalid email or password.");
                return Result<IdentityUserInfo>.Failure(error);
            }

            var roles = await _userManager.GetRolesAsync(user);
            var identityUserInfo = new IdentityUserInfo
                (user.Id,
                user.FirstName,
                user.LastName,
                user.Email!,
                user.IsActive,
                roles);
            return Result<IdentityUserInfo>.Success(identityUserInfo);
        }
    }
}
