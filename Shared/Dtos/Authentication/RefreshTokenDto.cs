namespace LibraryManagement.Shared.Dtos.Authentication
{
    public record class RefreshTokenDto
        (string TokenHash,
        string UserId,
        DateTime ExpiresAt,
        DateTime? RevokedAt,
        string? ReplacedByTokenHash)
    {
        public bool IsActive => RevokedAt is null && ExpiresAt >= DateTime.UtcNow;
    }
}
