namespace LibraryManagement.Persistence.Email
{
    internal sealed class MailKitEmailService : IEmailService
    {
        private readonly EmailOptions _options;
        private readonly ILogger<MailKitEmailService> _logger;

        public MailKitEmailService(
            IOptions<EmailOptions> options,
            ILogger<MailKitEmailService> logger)
        {
            _options = options.Value;
            _logger = logger;
        }

        public async Task SendEmailConfirmationAsync(string email, string callbackUrl)
        {
            var body = $"""
            <h1>Welcome to Library Management</h1>
            <p>Please confirm your email address by clicking the link below:</p>
            <p><a href="{callbackUrl}">Confirm my email</a></p>
            <p>If you did not create an account, you can safely ignore this email.</p>
            """;

            await SendAsync(email, "Confirm your Library Management account", body);
        }

        public async Task SendPasswordResetAsync(string email, string callbackUrl)
        {
            var body = $"""
            <h1>Password reset request</h1>
            <p>We received a request to reset your password. Click the link below to choose a new one:</p>
            <p><a href="{callbackUrl}">Reset my password</a></p>
            <p>If you did not request this, you can safely ignore this email.</p>
            """;

            await SendAsync(email, "Reset your Library Management password", body);
        }

        private async Task SendAsync(string to, string subject, string htmlBody)
        {
            // لو الـ secrets مش متظبطة في الـ dev، منوقعش التطبيق
            if (string.IsNullOrWhiteSpace(_options.SmtpServer))
            {
                _logger.LogWarning(
                    "Email provider is not configured. Skipping email to {Email}.", to);
                return;
            }

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_options.SenderName, _options.SenderEmail));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;

            message.Body = new BodyBuilder
            {
                HtmlBody = htmlBody
            }.ToMessageBody();

            var socketOptions = _options.Port == 465
            ? SecureSocketOptions.SslOnConnect
            : SecureSocketOptions.StartTls;

            using var client = new SmtpClient();

            await client.ConnectAsync(_options.SmtpServer, _options.Port, socketOptions);
            await client.AuthenticateAsync(_options.Username, _options.Password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            _logger.LogInformation("Email sent to {Email} with subject {Subject}", to, subject);
        }
    }
}
