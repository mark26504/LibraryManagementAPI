namespace LibraryManagement.Persistence.Identity.Store
{
    internal sealed class RefreshTokenStore : IRefreshTokenStore
    {
        private readonly ApplicationDbContext _dbContext;

        public RefreshTokenStore(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task StoreTokenAsync(string userId, string tokenHash, DateTime expiresAt)
        {
            var refreshToken = new RefreshToken
            {
                UserId = userId,
                TokenHash = tokenHash,
                ExpiresAt = expiresAt
            };

            _dbContext.RefreshTokens.Add(refreshToken);
            return _dbContext.SaveChangesAsync();
        }

        public async Task<RefreshTokenDto?> GetTokenByHashAsync(string tokenHash)
        {
            var refreshToken = await _dbContext.RefreshTokens.FirstOrDefaultAsync
                (rt => rt.TokenHash == tokenHash);
            
            if (refreshToken == null) return null;

            return new RefreshTokenDto
                (refreshToken.TokenHash,
                refreshToken.UserId,
                refreshToken.ExpiresAt,
                refreshToken.RevokedAt,
                refreshToken.ReplacedByTokenHash
                );
        }

        public async Task RevokeTokenAsync(string tokenHash, string? replacedByTokenHash = null)
        {
            var refreshToken = await _dbContext.RefreshTokens.FirstOrDefaultAsync(rt => rt.TokenHash == tokenHash);
            if (refreshToken == null) return;

            refreshToken.RevokedAt = DateTime.UtcNow;
            refreshToken.ReplacedByTokenHash = replacedByTokenHash;
            
            await _dbContext.SaveChangesAsync();
        }

    }
}
