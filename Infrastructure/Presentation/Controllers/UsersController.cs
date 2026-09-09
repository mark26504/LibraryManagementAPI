using LibraryManagement.Shared.Constants;

namespace LibraryManagement.Presentation.Controllers
{
    [ApiController]
    [Route("api/v1/users")]
    public class UsersController : ApiControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: /api/v1/users
        [HttpGet]
        [Authorize(Roles = RoleNames.Admin)]
        public async Task<IActionResult> GetAllUsers(
            [FromQuery] UserQueryParametersDto queryParameters)
        {
            var result = await _userService.GetAllUsersAsync(queryParameters);
            return result.IsSuccess ? Ok(result.Value) : Failure(result);
        }

        // GET: /api/v1/users/{id}
        [HttpGet("{id}")]
        [Authorize(Roles = RoleNames.Admin)]
        public async Task<IActionResult> GetUserById([FromRoute] string id)
        {
            var result = await _userService.GetUserByIdAsync(id);
            return result.IsSuccess ? Ok(result.Value) : Failure(result);
        }

        // PATCH: /api/v1/users/{id}/status
        [HttpPatch("{id}/status")]
        [Authorize(Roles = RoleNames.Admin)]
        public async Task<IActionResult> UpdateUserStatus(
            [FromRoute] string id,
            [FromBody] UpdateUserStatusDto statusDto)
        {
            var result = await _userService.UpdateUserStatusAsync(id, statusDto);
            return result.IsSuccess ? Ok(result.Value) : Failure(result);
        }

        // POST: /api/v1/users/{id}/roles
        [HttpPost("{id}/roles")]
        [Authorize(Roles = RoleNames.Admin)]
        public async Task<IActionResult> UpdateUserRoles(
            [FromRoute] string id,
            [FromBody] UpdateUserRolesDto rolesDto)
        {
            var actorId = User.FindFirstValue("sub") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(actorId))
                return Unauthorized();

            var result = await _userService.UpdateUserRolesAsync(actorId, id, rolesDto);
            return result.IsSuccess ? Ok(result.Value) : Failure(result);
        }

        // PATCH: /api/v1/users/me/profile
        [HttpPatch("me/profile")]
        public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateProfileDto profileDto)
        {
            var userId = User.FindFirstValue("sub") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var result = await _userService.UpdateUserProfileAsync(userId, profileDto);
            return result.IsSuccess ? Ok(result.Value) : Failure(result);
        }
    }
}