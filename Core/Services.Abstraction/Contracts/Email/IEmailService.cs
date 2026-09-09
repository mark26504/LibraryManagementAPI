namespace LibraryManagement.Services.Abstraction.Contracts.Email
{
    public interface IEmailService
    {
        Task SendEmailConfirmationAsync(string email, string token);
        Task SendPasswordResetAsync(string email, string token);
    }
}
