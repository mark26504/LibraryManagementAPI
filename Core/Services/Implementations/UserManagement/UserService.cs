namespace LibraryManagement.Services.Implementations.UserManagement
{
    internal sealed class UserService : IUserService
    {
        private readonly IIdentityManager _identityManager;
        private readonly IMapper _mapper;

        public UserService(IIdentityManager identityManager, IMapper mapper)
        {
            _identityManager = identityManager;
            _mapper = mapper;
        }
        public async Task<Result<PagedResponse<UserDto>>> GetAllUsersAsync(UserQueryParametersDto queryParameters)
        {
            var users = await _identityManager.GetUsersAsync(queryParameters);
            if (users.IsFailure)
                return Result<PagedResponse<UserDto>>.Failure(users.Error);

            var mappedUsers = _mapper.Map<IEnumerable<UserDto>>(users.Value.Items);
            var mappedPagedResponse = new PagedResponse<UserDto>(
                                                    mappedUsers,
                                                    users.Value.TotalCount,
                                                    users.Value.PageNumber,
                                                    users.Value.PageSize);
            return Result<PagedResponse<UserDto>>.Success(mappedPagedResponse);
        }

        public async Task<Result<UserDto>> GetUserByIdAsync(string userId)
        {
            var userResult = await _identityManager.GetUserByIdAsync(userId);
            if (userResult.IsFailure)
                return Result<UserDto>.Failure(userResult.Error);

            var mappedUser = _mapper.Map<UserDto>(userResult.Value);
            return Result<UserDto>.Success(mappedUser);
        }

        public async Task<Result> UpdateUserProfileAsync(string userId, UpdateProfileDto profileDto)
        {
            var updateResult = await _identityManager.UpdateUserProfileAsync(userId, profileDto.FirstName, profileDto.LastName);
            if (updateResult.IsFailure)
                return Result.Failure(updateResult.Error);
            return Result.Success();
        }

        public async Task<Result> UpdateUserRolesAsync(string userId, UpdateUserRolesDto rolesDto)
        {
            var updateResult = await _identityManager.UpdateUserRolesAsync(userId, rolesDto.Roles);
            if (updateResult.IsFailure)
                return Result.Failure(updateResult.Error);
            return Result.Success();
        }

        public async Task<Result> UpdateUserStatusAsync(string userId, UpdateUserStatusDto statusDto)
        {
            var updateResult = await _identityManager.UpdateUserStatusAsync(userId, statusDto.IsActive);
            if (updateResult.IsFailure)
                return Result.Failure(updateResult.Error);
            return Result.Success();
        }
    }
}
