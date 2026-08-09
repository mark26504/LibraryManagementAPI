namespace LibraryManagement.Persistence.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        #region ApplicationUser -> BorrowingRecord [1:M]
        // Nav Prop
        public ICollection<BorrowingRecord> BorrowingRecords { get; set; } = new List<BorrowingRecord>();

        #endregion

        #region ApplicationUser -> RefreshToken [1:M]
        // Nav Prop
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

        #endregion
    }
}
