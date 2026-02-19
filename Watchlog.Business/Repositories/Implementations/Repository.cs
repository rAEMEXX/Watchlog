using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Watchlog.Business.Repositories.Interfaces;
using Watchlog.Data.Persistance; // adjust if your DbContext namespace differs

namespace Watchlog.Business.Repositories.Implementations
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private readonly ApplicationDbContext _db;
        private readonly DbSet<TEntity> _set;

        public Repository(ApplicationDbContext db)
        {
            _db = db;
            _set = _db.Set<TEntity>();
        }

        public IQueryable<TEntity> Query()
            => _set.AsQueryable();

        public async Task<TEntity?> GetByIdAsync(object id, CancellationToken ct = default)
            => await _set.FindAsync(new[] { id }, ct);

        public async Task<List<TEntity>> GetAllAsync(CancellationToken ct = default)
            => await _set.ToListAsync(ct);

        public async Task<List<TEntity>> FindAsync(
            Expression<Func<TEntity, bool>> predicate,
            CancellationToken ct = default)
            => await _set.Where(predicate).ToListAsync(ct);

        public async Task AddAsync(TEntity entity, CancellationToken ct = default)
            => await _set.AddAsync(entity, ct);

        public async Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken ct = default)
            => await _set.AddRangeAsync(entities, ct);

        public void Update(TEntity entity)
            => _set.Update(entity);

        public void Delete(TEntity entity)
            => _set.Remove(entity);

        public void DeleteRange(IEnumerable<TEntity> entities)
            => _set.RemoveRange(entities);

        public Task<int> SaveChangesAsync(CancellationToken ct = default)
            => _db.SaveChangesAsync(ct);
    }
}