namespace LibraryManagement.Services.Abstraction.Contracts.Authentication
{
    public interface IRefreshTokenStore
    {
        Task StoreTokenAsync(string userId, string tokenHash, DateTime expiresAt);
        Task<RefreshTokenDto?> GetTokenByHashAsync(string tokenHash);
        Task RevokeTokenAsync(string tokenHash, string? replacedByTokenHash = null);
    }
}
