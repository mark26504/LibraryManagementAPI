namespace LibraryManagement.Services.Implementations.UserManagement
{
    internal sealed class UserService : IUserService
    {
        private readonly IIdentityManager _identityManager;
        private readonly IMapper _mapper;

        private static readonly string[] KnownRoles =
        {
            RoleNames.Admin,
            RoleNames.Librarian,
            RoleNames.Member
        };
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
                return Result<UserDto>.Failure(
                    Error.NotFound("User.NotFound", "User not found."));

            var mappedUser = _mapper.Map<UserDto>(userResult.Value);
            return Result<UserDto>.Success(mappedUser);
        }

        public async Task<Result<UserDto>> UpdateUserStatusAsync(string userId, UpdateUserStatusDto statusDto)
        {
            var current = await _identityManager.GetUserByIdAsync(userId);
            if (current.IsFailure)
                return Result<UserDto>.Failure(
                    Error.NotFound("User.NotFound", "User not found."));

            var currentUser = _mapper.Map<UserDto>(current.Value);

            if (!statusDto.IsActive && await IsLastActiveAdminAsync(currentUser))
                return Result<UserDto>.Failure(
                    Error.Conflict("User.LastAdmin", "The last active administrator cannot be deactivated."));

            var updateResult = await _identityManager.UpdateUserStatusAsync(userId, statusDto.IsActive);
            if (updateResult.IsFailure)
                return Result<UserDto>.Failure(updateResult.Error);

            return await GetUserByIdAsync(userId);
        }

        public async Task<Result<UserDto>> UpdateUserRolesAsync(string actorId, string userId, UpdateUserRolesDto rolesDto)
        {
            if (rolesDto.Roles is null || !rolesDto.Roles.Any())
                return Result<UserDto>.Failure(
                    Error.Validation("User.RolesRequired", "A user must retain at least one role."));

            var unknownRoles = rolesDto.Roles.Where(r => !KnownRoles.Contains(r)).ToList();
            if (unknownRoles.Any())
                return Result<UserDto>.Failure(
                    Error.Validation("User.UnknownRole", $"Unknown role(s): {string.Join(", ", unknownRoles)}."));

            if (string.Equals(actorId, userId, StringComparison.Ordinal))
                return Result<UserDto>.Failure(
                    Error.Forbidden("User.SelfRoleModification", "You cannot modify your own roles."));

            var current = await _identityManager.GetUserByIdAsync(userId);
            if (current.IsFailure)
                return Result<UserDto>.Failure(
                    Error.NotFound("User.NotFound", "User not found."));

            var currentUser = _mapper.Map<UserDto>(current.Value);

            var removesAdmin = currentUser.Roles.Contains(RoleNames.Admin) && !rolesDto.Roles.Contains(RoleNames.Admin);
            if (removesAdmin && await IsLastActiveAdminAsync(currentUser))
                return Result<UserDto>.Failure(
                    Error.Conflict("User.LastAdmin", "The last active administrator cannot lose the Admin role."));

            var updateResult = await _identityManager.UpdateUserRolesAsync(userId, rolesDto.Roles);
            if (updateResult.IsFailure)
                return Result<UserDto>.Failure(updateResult.Error);

            return await GetUserByIdAsync(userId);
        }

        public async Task<Result<UserDto>> UpdateUserProfileAsync(string userId, UpdateProfileDto profileDto)
        {
            var current = await _identityManager.GetUserByIdAsync(userId);
            if (current.IsFailure)
                return Result<UserDto>.Failure(
                    Error.NotFound("User.NotFound", "User not found."));

            var updateResult = await _identityManager.UpdateUserProfileAsync(userId, profileDto.FirstName, profileDto.LastName);
            if (updateResult.IsFailure)
                return Result<UserDto>.Failure(updateResult.Error);

            return await GetUserByIdAsync(userId);
        }

        #region Helper Methods

        private async Task<bool> IsLastActiveAdminAsync(UserDto user)
        {
            if (!user.IsActive || !user.Roles.Contains("Admin"))
                return false;

            var admins = await _identityManager.GetUsersAsync(new UserQueryParametersDto
            {
                Role = "Admin",
                IsActive = true,
                PageNumber = 1,
                PageSize = 1
            });

            if (admins.IsFailure)
                return false;

            return admins.Value.TotalCount <= 1;
        }

        #endregion
    }
}