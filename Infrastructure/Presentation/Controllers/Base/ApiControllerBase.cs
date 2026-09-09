using LibraryManagement.Shared.Responses;

namespace LibraryManagement.Presentation.Controllers.Base
{
    public abstract class ApiControllerBase : ControllerBase
    {
        protected IActionResult Failure(Result result)
        {
            if (result.IsSuccess)
                throw new InvalidOperationException("Cannot handle a successful result.");

            var error = result.Error;

            return error.Type switch
            {
                ErrorType.NotFound => NotFound(CreateProblemDetails(
                    StatusCodes.Status404NotFound,
                    "Not Found",
                    error.Message)),

                ErrorType.Conflict => Conflict(CreateProblemDetails(
                    StatusCodes.Status409Conflict,
                    "Conflict",
                    error.Message)),

                ErrorType.Validation => BadRequest(CreateProblemDetails(
                    StatusCodes.Status400BadRequest,
                    "Validation failed",
                    error.Message)),

                ErrorType.Forbidden => StatusCode(StatusCodes.Status403Forbidden, CreateProblemDetails(
                    StatusCodes.Status403Forbidden,
                    "Forbidden",
                    error.Message)),

                ErrorType.Unauthorized => Unauthorized(CreateProblemDetails(
                    StatusCodes.Status401Unauthorized,
                    "Unauthorized",
                    error.Message)),

                _ => BadRequest(CreateProblemDetails(
                    StatusCodes.Status400BadRequest,
                    "Bad Request",
                    error.Message))
            };
        }

        #region Helper Method
        private ProblemDetails CreateProblemDetails(int status, string title, string detail)
        {
            return new ProblemDetails
            {
                Status = status,
                Title = title,
                Detail = detail,
                Instance = HttpContext.Request.Path,
                Type = $"https://httpstatuses.io/{status}",
                Extensions =
            {
                ["traceId"] = HttpContext.TraceIdentifier
            }
            };
        }

        #endregion
    }
}
