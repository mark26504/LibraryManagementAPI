namespace LibraryManagement.Persistence.Identity.Managers
{
    internal sealed class IdentityManager : IIdentityManager
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public IdentityManager(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        #region Authentication

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
            var roleResult = await _userManager.AddToRoleAsync(user, role);

            if (!roleResult.Succeeded)
            {
                var identityError = roleResult.Errors.First();

                var error = new Error(
                    identityError.Code,
                    identityError.Description);

                return Result.Failure(error);
            }

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
        #endregion

        #region User Management

        public async Task<Result<PagedResponse<IdentityUserInfo>>> GetUsersAsync(UserQueryParametersDto queryParameters)
        {
            if (queryParameters == null) 
                return Result<PagedResponse<IdentityUserInfo>>.Failure(new Error("QueryParameters.Null", "Query parameters cannot be null."));
            
            var query = _userManager.Users.AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(queryParameters.Search))
                query = query.Where(u => u.FirstName.Contains(queryParameters.Search) || u.LastName.Contains(queryParameters.Search));
            
            if (queryParameters.IsActive.HasValue)
                query = query.Where(u => u.IsActive == queryParameters.IsActive.Value);

            // Role Filtering
            if (!string.IsNullOrEmpty(queryParameters.Role))
            {
                var userIdsInRole = (await _userManager.GetUsersInRoleAsync(queryParameters.Role))
                    .Select(u => u.Id)
                    .ToList();
                query = query.Where(u => userIdsInRole.Contains(u.Id));
            }

            // Sorting
            if (string.IsNullOrEmpty(queryParameters.SortBy))
                query = query.OrderBy(u => u.CreatedAt);
            if (!string.IsNullOrEmpty(queryParameters.SortBy))
            { 
                var sortBy = queryParameters.SortBy.ToLower();
                var sortByDirection = queryParameters.SortDirection?.ToLower() ?? "asc";

                query = sortBy switch
                {
                    "firstname" => sortByDirection == "desc" ? query.OrderByDescending(u => u.FirstName) : query.OrderBy(u => u.FirstName),
                    "lastname" => sortByDirection == "desc" ? query.OrderByDescending(u => u.LastName) : query.OrderBy(u => u.LastName),
                    "email" => sortByDirection == "desc" ? query.OrderByDescending(u => u.Email) : query.OrderBy(u => u.Email),
                    _ => query.OrderBy(u => u.Id).ThenBy(u => u.Email)
                };
            }

            // Pagination
            var totalCount = await query.CountAsync();
            query = query.Skip((queryParameters.PageNumber - 1) * queryParameters.PageSize)
                         .Take(queryParameters.PageSize);
            var users = await query.ToListAsync();
            var identityUsers = new List<IdentityUserInfo>();
            foreach (var user in users) {
                var roles = await _userManager.GetRolesAsync(user);
                var identityUserInfo = new IdentityUserInfo(
                    user.Id,
                    user.FirstName,
                    user.LastName,
                    user.Email!,
                    user.IsActive,
                    roles
                );
                identityUsers.Add(identityUserInfo);
            }
            var pagedResponse = new PagedResponse<IdentityUserInfo>(
                identityUsers,
                totalCount,
                queryParameters.PageNumber,
                queryParameters.PageSize);
            return Result<PagedResponse<IdentityUserInfo>>.Success(pagedResponse);
        }

        public async Task<Result> UpdateUserProfileAsync(string userId, string firstName, string lastName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                return Result.Failure(new Error("User.NotFound", "User not found."));
            
            user.FirstName = firstName;
            user.LastName = lastName;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var identityErrors = result.Errors.First();
                var error = new Error(identityErrors.Code, identityErrors.Description);
                return Result.Failure(error);
            }

            return Result.Success();
        }

        public async Task<Result> UpdateUserRolesAsync(string userId, IEnumerable<string> roles)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                return Result.Failure(new Error("User.NotFound", "User not found."));

            var currentRoles = await _userManager.GetRolesAsync(user);
            
            var removeRolesResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
            if (!removeRolesResult.Succeeded)
            {
                var identityErrors = removeRolesResult.Errors.First();
                var error = new Error(identityErrors.Code, identityErrors.Description);
                return Result.Failure(error);
            }

            var addRolesResult = await _userManager.AddToRolesAsync(user, roles);
            if (!addRolesResult.Succeeded)
            {
                var identityErrors = addRolesResult.Errors.First();
                var error = new Error(identityErrors.Code, identityErrors.Description);
                return Result.Failure(error);
            }

            return Result.Success();
        }

        public async Task<Result> UpdateUserStatusAsync(string userId, bool isActive)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                return Result.Failure(new Error("User.NotFound", "User not found."));

            user.IsActive = isActive;
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var identityErrors = result.Errors.First();
                var error = new Error(identityErrors.Code, identityErrors.Description);
                return Result.Failure(error);
            }

            return Result.Success();
        }

        #endregion

        #region Email Confirmation & Password Recovery

        public async Task<Result<string>> FindUserIdByEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
                return Result<string>.Failure(
                    new Error("Identity.UserNotFound", "User not found."));

            return Result<string>.Success(user.Id);
        }

        public async Task<Result<string>> GenerateEmailConfirmationTokenAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                return Result<string>.Failure(
                    new Error("Identity.UserNotFound", "User not found."));

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            return Result<string>.Success(token);
        }

        public async Task<Result> ConfirmEmailAsync(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                return Result.Failure(
                    new Error("Identity.UserNotFound", "User not found."));

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
                return Result.Failure(
                    Error.Validation("Identity.InvalidToken", "Invalid or expired confirmation token."));

            return Result.Success();
        }

        public async Task<Result<string>> GeneratePasswordResetTokenAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                return Result<string>.Failure(
                    new Error("Identity.UserNotFound", "User not found."));

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            return Result<string>.Success(token);
        }

        public async Task<Result> ResetPasswordAsync(string userId, string token, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                return Result.Failure(
                    new Error("Identity.UserNotFound", "User not found."));

            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
            if (!result.Succeeded)
                return Result.Failure(
                    Error.Validation("Identity.InvalidToken", "Invalid or expired reset token or weak password."));

            return Result.Success();
        }

        #endregion 
    }
}