namespace LibraryManagement.Services.Implementations.Authentication
{
    internal sealed class AuthenticationService : IAuthenticationService
    {
        private readonly IIdentityManager _identityManager;
        private readonly ITokenProvider _tokenProvider;
        private readonly IRefreshTokenStore _refreshTokenStore;
        private readonly IOptions<JwtOptions> _jwtOptions;

        public AuthenticationService(
            IIdentityManager identityManager,
            ITokenProvider tokenProvider,
            IRefreshTokenStore refreshTokenStore,
            IOptions<JwtOptions> jwtOptions)
        {
            _identityManager = identityManager;
            _tokenProvider = tokenProvider;
            _refreshTokenStore = refreshTokenStore;
            _jwtOptions = jwtOptions;
        }

        public async Task<Result> RegisterUserAsync(UserRegistrationDto registrationDto)
        {
            var result = await _identityManager.CreateUserAsync(registrationDto, "Member");
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

            var authenticatedUser = new AuthenticationUserDto(
                result.Value.Id,
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
                accessToken.Token,
                accessToken.ExpiresAt,
                authenticatedUser);

            var authenticationResult = new AuthenticationResult(authenticationResponse, rawRefreshToken);
            return Result<AuthenticationResult>.Success(authenticationResult);
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
                accessToken.Token,
                accessToken.ExpiresAt,
                authenticatedUser);

            var authenticationResult = new AuthenticationResult(authenticationResponse, newRawRefreshToken);
            return Result<AuthenticationResult>.Success(authenticationResult);
        }

        public async Task<Result> RevokeTokenAsync(string refreshToken)
        {
            var hashedToken = _tokenProvider.HashRefreshToken(refreshToken);
            await _refreshTokenStore.RevokeTokenAsync(hashedToken);
            return Result.Success();
        }
    }
}