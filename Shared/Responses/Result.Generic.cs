namespace LibraryManagement.Shared.Responses
{
    public class Result<T> : Result
    {
        public T Value { get; }
        protected internal Result(bool isSuccess, Error error, T value) : base(isSuccess, error)
        {
            Value = value;
        }

        public static Result<T> Success(T value) => new Result<T>(true, Error.None, value);
        public static new Result<T> Failure(Error error) => new Result<T>(false, error, default(T)!);
    }
}
