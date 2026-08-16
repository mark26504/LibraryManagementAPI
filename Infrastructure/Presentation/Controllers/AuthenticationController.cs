namespace LibraryManagement.Presentation.Controllers
{
    [ApiController]
    [Route("api/v1/auth")]
    public class AuthenticationController : ControllerBase
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

        [HttpPost("register")]
        public async Task<IActionResult> Register(
            [FromBody] UserRegistrationDto registrationDto)
        {
            var result =
                await _authenticationService.RegisterUserAsync(
                    registrationDto);

            if (result.IsFailure)
                return BadRequest(result.Error);

            return Ok();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(
            [FromBody] UserLoginDto loginDto)
        {
            var result =
                await _authenticationService.LoginAsync(loginDto);

            if (result.IsFailure)
                return BadRequest(result.Error);

            SetRefreshTokenCookie(
                result.Value.RefreshToken);

            return Ok(result.Value.Response);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken()
        {
            if (!Request.Cookies.TryGetValue(
                    RefreshTokenCookieName,
                    out var refreshToken) ||
                string.IsNullOrWhiteSpace(refreshToken))
            {
                return Unauthorized();
            }

            var result =
                await _authenticationService.RefreshTokenAsync(
                    refreshToken);

            if (result.IsFailure)
                return Unauthorized(result.Error);

            SetRefreshTokenCookie(
                result.Value.RefreshToken);

            return Ok(result.Value.Response);
        }

        [HttpPost("revoke-token")]
        public async Task<IActionResult> RevokeToken()
        {
            if (!Request.Cookies.TryGetValue(
                    RefreshTokenCookieName,
                    out var refreshToken) ||
                string.IsNullOrWhiteSpace(refreshToken))
            {
                return BadRequest(
                    "No refresh token found in cookies.");
            }

            var result =
                await _authenticationService.RevokeTokenAsync(
                    refreshToken);

            if (result.IsFailure)
                return BadRequest(result.Error);

            DeleteRefreshTokenCookie();

            return Ok();
        }

        #region Cookie Helpers

        private void SetRefreshTokenCookie(
            string refreshToken)
        {
            var expirationDays =
                _configuration.GetValue<int>(
                    "Jwt:RefreshTokenExpirationDays");

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddDays(
                    expirationDays),

                Path = "/api/v1/auth"
            };

            Response.Cookies.Append(
                RefreshTokenCookieName,
                refreshToken,
                cookieOptions);
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

            Response.Cookies.Delete(
                RefreshTokenCookieName,
                cookieOptions);
        }

        #endregion
    }
}