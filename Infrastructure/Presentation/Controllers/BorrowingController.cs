namespace LibraryManagement.Presentation.Controllers
{
    [ApiController]
    [Route("api/v1/borrowings")]
    [Authorize]
    public class BorrowingController : ApiControllerBase 
    {
        private readonly IBorrowingService _borrowingService;

        public BorrowingController(IBorrowingService borrowingService)
        {
            _borrowingService = borrowingService;
        }

        // POST: api/v1/borrowings
        [HttpPost]
        public async Task<IActionResult> BorrowBook([FromBody] CreateBorrowingRequest request)
        {
            var userId = User.FindFirstValue("sub") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var result = await _borrowingService.BorrowBookAsync(userId, request);

            if (result.IsSuccess)
                return Ok(result.Value);

            return Failure(result); 
        }

        // POST: api/v1/borrowings/{id}/return
        [HttpPost("{id:guid}/return")]
        public async Task<IActionResult> ReturnBook(Guid id)
        {
            var userId = User.FindFirstValue("sub") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            bool isStaff = User.IsInRole("Admin") || User.IsInRole("Librarian");
            var result = await _borrowingService.ReturnBookAsync(id, userId, isStaff);

            if (result.IsSuccess)
                return Ok(result.Value);

            return Failure(result);
        }

        // GET: api/v1/borrowings/my
        [HttpGet("my")]
        public async Task<IActionResult> GetMyBorrowings([FromQuery] BorrowingParameters parameters)
        {
            var userId = User.FindFirstValue("sub") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var result = await _borrowingService.GetMyBorrowingsAsync(userId, parameters);
            return result.IsSuccess ? Ok(result.Value) : Failure(result);
        }

        // GET: api/v1/borrowings/my/{id}
        [HttpGet("my/{id:guid}")]
        public async Task<IActionResult> GetMyBorrowingById(Guid id)
        {
            var userId = User.FindFirstValue("sub") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var result = await _borrowingService.GetMyBorrowingByIdAsync(id, userId);
            return result.IsSuccess ? Ok(result.Value) : Failure(result);
        }

        // ==========================================
        // STAFF OPERATIONS
        // ==========================================

        // GET: api/v1/borrowings
        [HttpGet]
        [Authorize(Roles = RoleNames.Admin + "," + RoleNames.Librarian)]
        public async Task<IActionResult> GetAllBorrowings([FromQuery] BorrowingParameters parameters)
        {
            var result = await _borrowingService.GetAllBorrowingsAsync(parameters);
            return result.IsSuccess ? Ok(result.Value) : Failure(result);
        }

        // GET: api/v1/borrowings/{id}
        [HttpGet("{id:guid}")]
        [Authorize(Roles = RoleNames.Admin + "," + RoleNames.Librarian)]
        public async Task<IActionResult> GetBorrowingById(Guid id)
        {
            var result = await _borrowingService.GetBorrowingByIdAsync(id);
            return result.IsSuccess ? Ok(result.Value) : Failure(result);
        }

        // GET: api/v1/borrowings/overdue
        [HttpGet("overdue")]
        [Authorize(Roles = RoleNames.Admin + "," + RoleNames.Librarian)]
        public async Task<IActionResult> GetOverdueBorrowings([FromQuery] BorrowingParameters parameters)
        {
            var result = await _borrowingService.GetOverdueBorrowingsAsync(parameters);
            return result.IsSuccess ? Ok(result.Value) : Failure(result);
        }
    }
}