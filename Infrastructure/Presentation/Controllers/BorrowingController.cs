namespace LibraryManagement.Presentation.Controllers
{
    [ApiController]
    [Route("api/v1/borrowings")]
    [Authorize]
    public class BorrowingController : ControllerBase
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

            return BadRequest(result.Error);
        }

        // PUT: api/borrowing/{id}/return
        [HttpPut("{id:guid}/return")]
        public async Task<IActionResult> ReturnBook(Guid id)
        {
            var userId = User.FindFirstValue("sub") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            bool isStaff = User.IsInRole("Admin") || User.IsInRole("Librarian");

            var result = await _borrowingService.ReturnBookAsync(id, userId, isStaff);

            if (result.IsSuccess)
                return Ok(result.Value);

            return BadRequest(result.Error);
        }

        // GET: api/borrowing/my
        [HttpGet("my")]
        public async Task<IActionResult> GetMyBorrowings([FromQuery] BorrowingParameters parameters)
        {
            var userId = User.FindFirstValue("sub") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var result = await _borrowingService.GetMyBorrowingsAsync(userId, parameters);

            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
        }

        // GET: api/borrowing/my/{id}
        [HttpGet("my/{id:guid}")]
        public async Task<IActionResult> GetMyBorrowingById(Guid id)
        {
            var userId = User.FindFirstValue("sub") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var result = await _borrowingService.GetMyBorrowingByIdAsync(id, userId);

            return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
        }


        // ==========================================
        // STAFF OPERATIONS
        // ==========================================

        // GET: api/borrowing
        [HttpGet]
        [Authorize(Roles = "Admin, Librarian")] 
        public async Task<IActionResult> GetAllBorrowings([FromQuery] BorrowingParameters parameters)
        {
            var result = await _borrowingService.GetAllBorrowingsAsync(parameters);
            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
        }

        // GET: api/borrowing/{id}
        [HttpGet("{id:guid}")]
        [Authorize(Roles = "Admin, Librarian")]
        public async Task<IActionResult> GetBorrowingById(Guid id)
        {
            var result = await _borrowingService.GetBorrowingByIdAsync(id);
            return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
        }

        // GET: api/borrowing/overdue
        [HttpGet("overdue")]
        [Authorize(Roles = "Admin, Librarian")]
        public async Task<IActionResult> GetOverdueBorrowings([FromQuery] BorrowingParameters parameters)
        {
            var result = await _borrowingService.GetOverdueBorrowingsAsync(parameters);
            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
        }

        //[Authorize]
        //[HttpGet("debug/claims")]
        //public IActionResult DebugClaims()
        //    => Ok(User.Claims.Select(c => new { c.Type, c.Value }));
    }
}