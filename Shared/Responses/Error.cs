using System.Text.Json.Serialization;

namespace LibraryManagement.Shared.Responses
{
    public record Error(string Code, string Message) 
    {
        [JsonIgnore]
        public ErrorType Type { get; init; } = ErrorType.Failure;

        public static readonly Error None = new(string.Empty, string.Empty);

        public static Error Failure(string code, string message)
            => new(code, message) { Type = ErrorType.Failure };

        public static Error Validation(string code, string message)
            => new(code, message) { Type = ErrorType.Validation };

        public static Error NotFound(string code, string message)
            => new(code, message) { Type = ErrorType.NotFound };

        public static Error Conflict(string code, string message)
            => new(code, message) { Type = ErrorType.Conflict };

        public static Error Forbidden(string code, string message)
            => new(code, message) { Type = ErrorType.Forbidden };

        public static Error Unauthorized(string code, string message)
            => new(code, message) { Type = ErrorType.Unauthorized };
    };
}
