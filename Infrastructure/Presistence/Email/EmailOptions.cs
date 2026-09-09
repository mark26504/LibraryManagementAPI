namespace LibraryManagement.Persistence.Email
{
    public class EmailOptions
    {
        public const string SectionName = "EmailOptions";

        public string SmtpServer { get; set; } = string.Empty;
        public int Port { get; set; } = 587;
        public string SenderName { get; set; } = string.Empty;
        public string SenderEmail { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FrontendBaseUrl { get; set; } = "http://localhost:4200";

    }
}
