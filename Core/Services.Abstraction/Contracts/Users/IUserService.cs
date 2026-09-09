using LibraryManagement.Shared.Dtos.UserManagement;

namespace LibraryManagement.Services.Abstraction.Contracts.Users
{
    public interface IUserService
    {
        Task<Result<PagedResponse<UserDto>>> GetAllUsersAsync(UserQueryParametersDto queryParameters);
        Task<Result<UserDto>> GetUserByIdAsync(string userId);
        Task<Result<UserDto>> UpdateUserStatusAsync(string userId, UpdateUserStatusDto statusDto);
        Task<Result<UserDto>> UpdateUserRolesAsync(string actorId, string userId, UpdateUserRolesDto rolesDto);
        Task<Result<UserDto>> UpdateUserProfileAsync(string userId, UpdateProfileDto profileDto);
    }
}
