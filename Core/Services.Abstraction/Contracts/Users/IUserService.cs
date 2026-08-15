using LibraryManagement.Shared.Dtos.UserManagement;

namespace LibraryManagement.Services.Abstraction.Contracts.Users
{
    public interface IUserService
    {
        Task<Result<PagedResponse<UserDto>>> GetAllUsersAsync(UserQueryParametersDto queryParameters);
        Task<Result<UserDto>> GetUserByIdAsync(string userId);
        Task<Result> UpdateUserStatusAsync(string userId, UpdateUserStatusDto statusDto);
        Task<Result> UpdateUserRolesAsync(string userId, UpdateUserRolesDto rolesDto);
        Task<Result> UpdateUserProfileAsync(string userId, UpdateProfileDto profileDto);
    }
}
