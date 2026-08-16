namespace LibraryManagement.Presentation.Controllers
{
    [ApiController]
    [Route("api/v1/auth")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IConfiguration _configuration;

        public AuthenticationController(IAuthenticationService authenticationService, IConfiguration configuration)
        {
            _authenticationService = authenticationService;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegistrationDto userRegistrationDto)
        {
            var result = await _authenticationService.RegisterUserAsync(userRegistrationDto);
            if (result.IsFailure)
            {
                return BadRequest(result.Error);
            }
            return Ok();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDto userLoginDto)
        {
            var result = await _authenticationService.LoginAsync(userLoginDto);
            if(result.IsFailure)
            {
                return BadRequest(result.Error);
            }

            SetRefreshTokenCookie(result.Value.RefreshToken);
            return Ok(result.Value.Response);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken()
        {
            Request.Cookies.TryGetValue("refreshToken", out var refreshToken);
            if (refreshToken == null)
                return Unauthorized();

            var result = await _authenticationService.RefreshTokenAsync(refreshToken);
            if (result.IsFailure)
            {
                return Unauthorized(result.Error);
            }

            SetRefreshTokenCookie(result.Value.RefreshToken);
            return Ok(result.Value.Response);
        }

        [HttpPost("revoke-token")]
        public async Task<IActionResult> RevokeToken()
        {
            Request.Cookies.TryGetValue("refreshToken", out var refreshToken);
            if (refreshToken == null)
                return BadRequest("No refresh token found in cookies.");
            var result = await _authenticationService.RevokeTokenAsync(refreshToken);
            if (result.IsFailure)
            {
                return BadRequest(result.Error);
            }
            Response.Cookies.Delete("refreshToken");
            return Ok();
        }


        #region Helper Method

        private void SetRefreshTokenCookie(string refreshToken)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true, // Ensures it is only sent over HTTPS
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(
                    _configuration.GetValue<int>("Jwt:RefreshTokenExpirationDays"))
            };
            Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }

        #endregion
    }
}
