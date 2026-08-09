namespace LibraryManagement.Persistence.Identity
{
    public class RefreshToken
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string TokenHash { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime ExpiresAt { get; set; }
        public DateTime? RevokedAt { get; set; }
        public string? ReplacedByTokenHash { get; set; }

        #region ApplicationUser -> RefreshToken [1:M]

        // Nav Prop
        public ApplicationUser User { get; set; } = null!;

        // Fk
        public string UserId { get; set; } = null!;

        #endregion
    }
}
