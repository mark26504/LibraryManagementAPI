namespace LibraryManagement.Shared.Dtos.Authentication
{
    public record SendEmailConfirmationRequest(string Email);
    public record ConfirmEmailRequest(string Email, string Token);
    public record ForgotPasswordRequest(string Email);
    public record ResetPasswordRequest(string Email, string Token, string Password);
}
