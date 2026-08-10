namespace LibraryManagement.Persistence.Repositories
{
    internal sealed class BorrowingRepository : GenericRepository<BorrowingRecord>, IBorrowingRepository
    {
        public BorrowingRepository(ApplicationDbContext dbContext) : base(dbContext){ }

    }
}
