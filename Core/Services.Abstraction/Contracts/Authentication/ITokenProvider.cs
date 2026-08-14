namespace LibraryManagement.Services.Abstraction.Contracts.Authentication
{
    public interface ITokenProvider
    {
        (string Token, DateTime ExpiresAt) GenerateAccessToken(AuthenticationUserDto userDto);
        string GenerateRefreshToken();
        string HashRefreshToken(string refreshToken);   

    }
}
