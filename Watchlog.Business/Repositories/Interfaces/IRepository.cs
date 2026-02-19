using System.Linq.Expressions;

namespace Watchlog.Business.Repositories.Interfaces
{
    public interface IRepository<TEntity> where TEntity : class
    {
        IQueryable<TEntity> Query();

        Task<TEntity?> GetByIdAsync(object id, CancellationToken ct = default);

        Task<List<TEntity>> GetAllAsync(CancellationToken ct = default);

        Task<List<TEntity>> FindAsync(
            Expression<Func<TEntity, bool>> predicate,
            CancellationToken ct = default);

        Task AddAsync(TEntity entity, CancellationToken ct = default);

        Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken ct = default);

        void Update(TEntity entity);

        void Delete(TEntity entity);

        void DeleteRange(IEnumerable<TEntity> entities);

        Task<int> SaveChangesAsync(CancellationToken ct = default);
    }
}