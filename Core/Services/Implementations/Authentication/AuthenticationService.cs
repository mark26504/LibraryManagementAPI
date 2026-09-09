namespace LibraryManagement.Services.Implementations.Authentication
{
    internal sealed class AuthenticationService : IAuthenticationService
    {
        private readonly IIdentityManager _identityManager;
        private readonly ITokenProvider _tokenProvider;
        private readonly IRefreshTokenStore _refreshTokenStore;
        private readonly IEmailService _emailService;
        private readonly IOptions<JwtOptions> _jwtOptions;
        private readonly ILogger<AuthenticationService> _logger;

        public AuthenticationService
             (IIdentityManager identityManager,
              ITokenProvider tokenProvider,
              IRefreshTokenStore refreshTokenStore,
              IEmailService emailService,
              IOptions<JwtOptions> jwtOptions,
              ILogger<AuthenticationService> logger)
        {
            _identityManager = identityManager;
            _tokenProvider = tokenProvider;
            _refreshTokenStore = refreshTokenStore;
            _emailService = emailService;
            _jwtOptions = jwtOptions;
            _logger = logger;
        }

        public async Task<Result> RegisterUserAsync(UserRegistrationDto registrationDto)
        {
            var result = await _identityManager.CreateUserAsync(registrationDto, RoleNames.Member);
            if (result.IsFailure)
            {
                if (result.Error.Code.Contains("Duplicate", StringComparison.OrdinalIgnoreCase))
                    return Result.Failure(
                        Error.Conflict(result.Error.Code, result.Error.Message));

                return result;
            }

            return Result.Success();
        }

        public async Task<Result<AuthenticationResult>> LoginAsync(UserLoginDto loginDto)
        {
            var result = await _identityManager.ValidateCredentialsAsync(loginDto);
            if (result.IsFailure)
                return Result<AuthenticationResult>.Failure(
                    Error.Unauthorized(result.Error.Code, result.Error.Message));

            if (!result.Value.IsActive)
                return Result<AuthenticationResult>.Failure(
                    Error.Forbidden("Authentication.AccountDeactivated", "Account is deactivated."));

            var authenticatedUser = new AuthenticationUserDto
                (result.Value.Id,
                 result.Value.FirstName,
                 result.Value.LastName,
                 result.Value.Email,
                 result.Value.Roles);

            var accessToken = _tokenProvider.GenerateAccessToken(authenticatedUser);
            var rawRefreshToken = _tokenProvider.GenerateRefreshToken();
            var hashedToken = _tokenProvider.HashRefreshToken(rawRefreshToken);
            var expiresAt = DateTime.UtcNow.AddDays(_jwtOptions.Value.RefreshTokenExpirationDays);

            await _refreshTokenStore.StoreTokenAsync(authenticatedUser.Id, hashedToken, expiresAt);

            var authenticationResponse = new AuthenticationResponse(
                accessToken.Token, accessToken.ExpiresAt, authenticatedUser);

            return Result<AuthenticationResult>.Success(
                new AuthenticationResult(authenticationResponse, rawRefreshToken));
        }

        public async Task<Result<AuthenticationResult>> RefreshTokenAsync(string refreshToken)
        {
            var hashedToken = _tokenProvider.HashRefreshToken(refreshToken);
            var storedToken = await _refreshTokenStore.GetTokenByHashAsync(hashedToken);

            var invalidRefreshTokenError = Error.Unauthorized(
                "Authentication.InvalidRefreshToken",
                "Refresh token is invalid or expired.");

            if (storedToken is null || !storedToken.IsActive)
                return Result<AuthenticationResult>.Failure(invalidRefreshTokenError);

            if (storedToken.ExpiresAt < DateTime.UtcNow)
                return Result<AuthenticationResult>.Failure(invalidRefreshTokenError);

            var userResult = await _identityManager.GetUserByIdAsync(storedToken.UserId);
            if (userResult.IsFailure || !userResult.Value.IsActive)
                return Result<AuthenticationResult>.Failure(invalidRefreshTokenError);

            var authenticatedUser = new AuthenticationUserDto(
                userResult.Value.Id,
                userResult.Value.FirstName,
                userResult.Value.LastName,
                userResult.Value.Email,
                userResult.Value.Roles);

            var accessToken = _tokenProvider.GenerateAccessToken(authenticatedUser);
            var newRawRefreshToken = _tokenProvider.GenerateRefreshToken();
            var newHashedToken = _tokenProvider.HashRefreshToken(newRawRefreshToken);
            var expiresAt = DateTime.UtcNow.AddDays(_jwtOptions.Value.RefreshTokenExpirationDays);

            await _refreshTokenStore.StoreTokenAsync(userResult.Value.Id, newHashedToken, expiresAt);
            await _refreshTokenStore.RevokeTokenAsync(hashedToken, newHashedToken);

            var authenticationResponse = new AuthenticationResponse(
                accessToken.Token, accessToken.ExpiresAt, authenticatedUser);

            return Result<AuthenticationResult>.Success(
                new AuthenticationResult(authenticationResponse, newRawRefreshToken));
        }

        public async Task<Result> RevokeTokenAsync(string refreshToken)
        {
            var hashedToken = _tokenProvider.HashRefreshToken(refreshToken);
            await _refreshTokenStore.RevokeTokenAsync(hashedToken);
            return Result.Success();
        }

        #region Email Confirmation & Password Recovery

        public async Task<Result> SendEmailConfirmationAsync(string email)
        {
            var userResult = await _identityManager.FindUserIdByEmailAsync(email);
            if (userResult.IsFailure)
            {
                _logger.LogWarning(
                    "Email confirmation requested for unknown address.");
                return Result.Success();
            }

            var tokenResult = await _identityManager.GenerateEmailConfirmationTokenAsync(userResult.Value);
            if (tokenResult.IsFailure)
            {
                _logger.LogError(
                    "Failed to generate confirmation token for user {UserId}", userResult.Value);
                return Result.Success();
            }

            await _emailService.SendEmailConfirmationAsync(email, tokenResult.Value);
            return Result.Success();
        }

        public async Task<Result> ConfirmEmailAsync(string email, string token)
        {
            var userResult = await _identityManager.FindUserIdByEmailAsync(email);
            if (userResult.IsFailure)
                return Result.Failure(Error.Validation(
                    "Authentication.InvalidConfirmation",
                    "Invalid email or confirmation token."));

            var result = await _identityManager.ConfirmEmailAsync(userResult.Value, token);
            if (result.IsFailure)
                return Result.Failure(Error.Validation(
                    "Authentication.InvalidConfirmation",
                    "Invalid email or confirmation token."));

            return Result.Success();
        }

        public async Task<Result> ForgotPasswordAsync(string email)
        {
            var userResult = await _identityManager.FindUserIdByEmailAsync(email);
            if (userResult.IsFailure)
            {
                _logger.LogWarning(
                    "Password reset requested for unknown address.");
                return Result.Success();
            }

            var tokenResult = await _identityManager.GeneratePasswordResetTokenAsync(userResult.Value);
            if (tokenResult.IsFailure)
            {
                _logger.LogError(
                    "Failed to generate reset token for user {UserId}", userResult.Value);
                return Result.Success();
            }

            await _emailService.SendPasswordResetAsync(email, tokenResult.Value);
            return Result.Success();
        }

        public async Task<Result> ResetPasswordAsync(string email, string token, string newPassword)
        {
            var userResult = await _identityManager.FindUserIdByEmailAsync(email);
            if (userResult.IsFailure)
                return Result.Failure(Error.Validation(
                    "Authentication.InvalidReset",
                    "Invalid email or reset token."));

            var result = await _identityManager.ResetPasswordAsync(userResult.Value, token, newPassword);
            if (result.IsFailure)
                return Result.Failure(Error.Validation(
                    "Authentication.InvalidReset",
                    "Invalid email or reset token."));

            await _refreshTokenStore.RevokeAllActiveForUserAsync(userResult.Value);

            return Result.Success();
        }

        #endregion
    }
}