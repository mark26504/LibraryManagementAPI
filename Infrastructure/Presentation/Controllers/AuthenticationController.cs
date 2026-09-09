namespace LibraryManagement.Presentation.Controllers
{
    [ApiController]
    [Route("api/v1/auth")]
    public class AuthenticationController : ApiControllerBase
    {
        private const string RefreshTokenCookieName = "refreshToken";

        private readonly IAuthenticationService _authenticationService;
        private readonly IConfiguration _configuration;

        public AuthenticationController(
            IAuthenticationService authenticationService,
            IConfiguration configuration)
        {
            _authenticationService = authenticationService;
            _configuration = configuration;
        }

        // POST: api/v1/auth/register → 204 No Content per contract section 4
        [HttpPost("register")]
        public async Task<IActionResult> Register(
            [FromBody] UserRegistrationDto registrationDto)
        {
            var result = await _authenticationService.RegisterUserAsync(registrationDto);
            return result.IsSuccess ? NoContent() : Failure(result);
        }

        // POST: api/v1/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login(
            [FromBody] UserLoginDto loginDto)
        {
            var result = await _authenticationService.LoginAsync(loginDto);
            if (result.IsFailure)
                return Failure(result);

            SetRefreshTokenCookie(result.Value.RefreshToken);
            return Ok(result.Value.Response);
        }

        // POST: api/v1/auth/refresh-token  (body: {})
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken()
        {
            if (!Request.Cookies.TryGetValue(RefreshTokenCookieName, out var refreshToken) ||
                string.IsNullOrWhiteSpace(refreshToken))
            {
                return Failure(Result.Failure(
                    Error.Unauthorized("Authentication.NoRefreshToken", "No refresh token found in cookies.")));
            }

            var result = await _authenticationService.RefreshTokenAsync(refreshToken);
            if (result.IsFailure)
                return Failure(result);

            SetRefreshTokenCookie(result.Value.RefreshToken);
            return Ok(result.Value.Response);
        }

        // POST: api/v1/auth/revoke-token  (body: {})
        [HttpPost("revoke-token")]
        public async Task<IActionResult> RevokeToken()
        {
            if (!Request.Cookies.TryGetValue(RefreshTokenCookieName, out var refreshToken) ||
                string.IsNullOrWhiteSpace(refreshToken))
            {
                return Failure(Result.Failure(
                    Error.Unauthorized("Authentication.NoRefreshToken", "No refresh token found in cookies.")));
            }

            var result = await _authenticationService.RevokeTokenAsync(refreshToken);
            if (result.IsFailure)
                return Failure(result);

            DeleteRefreshTokenCookie();
            return Ok();
        }

        // POST: api/v1/auth/send-email-confirmation
        [HttpPost("send-email-confirmation")]
        public async Task<IActionResult> SendEmailConfirmation(
            [FromBody] SendEmailConfirmationRequest request)
        {
            var result = await _authenticationService.SendEmailConfirmationAsync(request.Email);
            return result.IsSuccess ? Ok() : Failure(result);
        }

        // POST: api/v1/auth/confirm-email
        [HttpPost("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(
            [FromBody] ConfirmEmailRequest request)
        {
            var result = await _authenticationService.ConfirmEmailAsync(request.Email, request.Token);
            return result.IsSuccess ? Ok() : Failure(result);
        }

        // POST: api/v1/auth/forgot-password
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(
            [FromBody] ForgotPasswordRequest request)
        {
            var result = await _authenticationService.ForgotPasswordAsync(request.Email);
            return result.IsSuccess ? Ok() : Failure(result);
        }

        // POST: api/v1/auth/reset-password
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(
            [FromBody] ResetPasswordRequest request)
        {
            var result = await _authenticationService.ResetPasswordAsync(
                request.Email, request.Token, request.Password);
            return result.IsSuccess ? Ok() : Failure(result);
        }

        #region Cookie Helpers

        private void SetRefreshTokenCookie(string refreshToken)
        {
            var expirationDays = _configuration.GetValue<int>("Jwt:RefreshTokenExpirationDays");

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddDays(expirationDays),
                Path = "/api/v1/auth"
            };

            Response.Cookies.Append(RefreshTokenCookieName, refreshToken, cookieOptions);
        }

        private void DeleteRefreshTokenCookie()
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Path = "/api/v1/auth"
            };

            Response.Cookies.Delete(RefreshTokenCookieName, cookieOptions);
        }

        #endregion
    }
}