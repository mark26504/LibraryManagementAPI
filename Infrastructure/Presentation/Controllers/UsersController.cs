namespace LibraryManagement.Presentation.Controllers
{
    [ApiController]
    [Route("api/v1/users")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: /api/v1/users
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUsers(
            [FromQuery] UserQueryParametersDto queryParameters)
        {
            var result =
                await _userService.GetAllUsersAsync(queryParameters);

            if (result.IsFailure)
                return BadRequest(result.Error);

            return Ok(result.Value);
        }

        // GET: /api/v1/users/{id}
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUserById(
            [FromRoute] string id)
        {
            var result =
                await _userService.GetUserByIdAsync(id);

            if (result.IsFailure)
                return NotFound(result.Error);

            return Ok(result.Value);
        }


        // PATCH: /api/v1/users/{id}/status
        [HttpPatch("{id}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUserStatus(
            [FromRoute] string id,
            [FromBody] UpdateUserStatusDto statusDto)
        {
            var result =
                await _userService.UpdateUserStatusAsync(id, statusDto);

            if (result.IsFailure)
                return BadRequest(result.Error);

            return NoContent();
        }

        // POST: /api/v1/users/{id}/roles
        [HttpPost("{id}/roles")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUserRoles(
            [FromRoute] string id,
            [FromBody] UpdateUserRolesDto rolesDto)
        {
            var result =
                await _userService.UpdateUserRolesAsync(id, rolesDto);

            if (result.IsFailure)
                return BadRequest(result.Error);

            return NoContent();
        }


        // PATCH: /api/v1/users/me/profile
        [HttpPatch("me/profile")]
        public async Task<IActionResult> UpdateMyProfile(
            [FromBody] UpdateProfileDto profileDto)
        {
            var userId =
                User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            var result =
                await _userService.UpdateUserProfileAsync(
                    userId,
                    profileDto);

            if (result.IsFailure)
                return BadRequest(result.Error);

            return NoContent();
        }
    }
}
