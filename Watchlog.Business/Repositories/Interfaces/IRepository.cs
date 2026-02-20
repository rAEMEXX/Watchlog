using System.Linq.Expressions;

namespace Watchlog.Business.Repositories.Interfaces;

public interface IRepository<T> where T : class
{
    IQueryable<T> Query();

    Task<T?> GetByIdAsync(Guid id, params Expression<Func<T, object>>[] includes);
    Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] includes);
    Task<IEnumerable<T>> FilterAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);

    Task AddAsync(T entity, CancellationToken ct = default);
    Task AddRangeAsync(IEnumerable<T> entities, CancellationToken ct = default);

    void Update(T entity);
    void Delete(T entity);
    void DeleteRange(IEnumerable<T> entities);

    Task<int> SaveChangesAsync(CancellationToken ct = default);
}