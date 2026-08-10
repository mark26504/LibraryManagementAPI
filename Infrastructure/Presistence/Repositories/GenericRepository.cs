using LibraryManagement.Persistence.Data;

namespace LibraryManagement.Persistence.Repositories
{
    internal abstract class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly ApplicationDbContext _dbContext;

        protected GenericRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<T> FindAll(bool trackChanges)
        {
            return trackChanges ?
                _dbContext.Set<T>() :
                _dbContext.Set<T>().AsNoTracking();

            ///IQueryable<T> query = _dbContext.Set<T>();
            ///if (!trackChanges) query = query.AsNoTracking();
            ///return query;
        }

        public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, bool trackChanges)
        {
            return trackChanges ?
                _dbContext.Set<T>().Where(expression) :
                _dbContext.Set<T>().Where(expression).AsNoTracking();
        }

        public void Create(T entity)
            => _dbContext.Set<T>().Add(entity);

        public void Delete(T entity)
            => _dbContext.Set<T>().Remove(entity);

        public void Update(T entity)
            => _dbContext.Set<T>().Update(entity);
    }
}
