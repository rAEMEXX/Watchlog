using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Watchlog.Business.Repositories.Interfaces;
using Watchlog.Data.Persistance;

namespace Watchlog.Business.Repositories.Implementations;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly ApplicationDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public IQueryable<T> Query() => _dbSet.AsQueryable();

    public async Task<T?> GetByIdAsync(Guid id, params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> q = _dbSet;
        foreach (var include in includes)
            q = q.Include(include);

        return await q.FirstOrDefaultAsync(e => EF.Property<Guid>(e, "Id") == id);
    }

    public async Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> q = _dbSet;
        foreach (var include in includes)
            q = q.Include(include);

        return await q.ToListAsync();
    }

    public async Task<IEnumerable<T>> FilterAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> q = _dbSet.Where(predicate);
        foreach (var include in includes)
            q = q.Include(include);

        return await q.ToListAsync();
    }

    public async Task AddAsync(T entity, CancellationToken ct = default) =>
        await _dbSet.AddAsync(entity, ct);

    public async Task AddRangeAsync(IEnumerable<T> entities, CancellationToken ct = default) =>
        await _dbSet.AddRangeAsync(entities, ct);

    public void Update(T entity) => _dbSet.Update(entity);
    public void Delete(T entity) => _dbSet.Remove(entity);
    public void DeleteRange(IEnumerable<T> entities) => _dbSet.RemoveRange(entities);

    public Task<int> SaveChangesAsync(CancellationToken ct = default) =>
        _context.SaveChangesAsync(ct);
}