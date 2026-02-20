using System.Linq.Expressions;

namespace Watchlog.Business.Repositories.Interfaces;

/// <summary>
/// Defines a generic repository interface for common data access operations.
/// </summary>
/// <typeparam name="T">The type of the entity.</typeparam>
public interface IRepository<T> where T : class
{
    /// <summary>
    /// Provides a queryable interface for the repository, allowing LINQ queries.
    /// </summary>
    /// <returns>An <see cref="IQueryable{T}"/> for the entity type.</returns>
    IQueryable<T> Query();

    /// <summary>
    /// Retrieves a single entity by its GUID.
    /// </summary>
    /// <param name="id">The unique identifier of the entity.</param>
    /// <param name="includes">Optional navigation properties to include.</param>
    /// <returns>The entity if found; otherwise, null.</returns>
    Task<T?> GetByIdAsync(Guid id, params Expression<Func<T, object>>[] includes);

    /// <summary>
    /// Retrieves all entities.
    /// </summary>
    /// <param name="includes">Optional navigation properties to include.</param>
    /// <returns>A collection of all entities.</returns>
    Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] includes);

    /// <summary>
    /// Retrieves entities matching the specified predicate.
    /// </summary>
    /// <param name="predicate">The filtering condition.</param>
    /// <param name="includes">Optional navigation properties to include.</param>
    /// <returns>A collection of matching entities.</returns>
    Task<IEnumerable<T>> FilterAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);

    /// <summary>
    /// Adds a new entity to the repository.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    /// <param name="ct">A cancellation token.</param>
    Task AddAsync(T entity, CancellationToken ct = default);

    /// <summary>
    /// Adds multiple entities to the repository.
    /// </summary>
    /// <param name="entities">The entities to add.</param>
    /// <param name="ct">A cancellation token.</param>
    Task AddRangeAsync(IEnumerable<T> entities, CancellationToken ct = default);

    /// <summary>
    /// Updates an existing entity.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    void Update(T entity);

    /// <summary>
    /// Removes an entity from the repository.
    /// </summary>
    /// <param name="entity">The entity to remove.</param>
    void Delete(T entity);

    /// <summary>
    /// Removes multiple entities from the repository.
    /// </summary>
    /// <param name="entities">The entities to remove.</param>
    void DeleteRange(IEnumerable<T> entities);

    /// <summary>
    /// Commits all changes made in the current unit of work.
    /// </summary>
    /// <param name="ct">A cancellation token.</param>
    /// <returns>The number of affected rows.</returns>
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}