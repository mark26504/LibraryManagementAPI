namespace LibraryManagement.Shared.Responses
{
    public record Error(string Code, string Message) 
    {
        public static readonly Error None = new Error(string.Empty, string.Empty);
    };
}
