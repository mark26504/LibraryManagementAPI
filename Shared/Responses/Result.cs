namespace LibraryManagement.Shared.Responses
{
    public class Result
    {
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public Error Error { get; }

        protected internal Result(bool isSuccess, Error error)
        {
            IsSuccess = isSuccess;
            Error = error;
        }

        public static Result Success() => new Result(true, Error.None);
        public static Result Failure(Error error) => new Result(false, error);
    }
}
