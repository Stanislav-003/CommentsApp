using backend.Database;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

/// <summary>
/// Base service class for CRUD operations on entities.
/// </summary>
public abstract class ServiceBase<T> where T : BaseModel
{
    protected readonly ApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the ServiceBase class with the specified database context.
    /// </summary>
    /// <param name="crmContext">The database context.</param>
    public ServiceBase(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Allows derived services to include related entities in queries.
    /// Override this method to specify which navigation properties should be eagerly loaded.
    /// </summary>
    protected virtual IQueryable<T> Include(DbSet<T> query)
    {
        return query;
    }

    /// <summary>
    /// Asynchronously adds a new entity to the database.
    /// </summary>
    /// <param name="object">The entity to add.</param>
    /// <returns>The added entity.</returns>
    public virtual async Task<T> AddAsync(T @object)
    {
        await _context.AddAsync(@object);
        return @object;
    }


    /// <summary>
    /// Asynchronously updates an existing entity in the database.
    /// </summary>
    /// <param name="updatedModel">The updated entity.</param>
    /// <returns>The updated entity, or null if not found.</returns>
    public virtual async Task<T?> UpdateAsync(T updatedModel)
    {
        _context.Update(updatedModel);
        return await GetById(updatedModel.Id);
    }

    /// <summary>
    /// Asynchronously deletes an entity by its identifier.
    /// </summary>
    /// <param name="id">The identifier of the entity to delete.</param>
    public virtual async Task DeleteById(Guid id)
    {
        var entity = await _context.Set<T>().FindAsync(id);
        if (entity != null)
        {
            _context.Remove(entity);
        }
    }

    /// <summary>
    /// Asynchronously retrieves an entity by its identifier.
    /// </summary>
    /// <param name="id">The identifier of the entity.</param>
    /// <returns>The entity if found; otherwise, null.</returns>
    public virtual async Task<T?> GetById(Guid? id, CancellationToken cancellationToken = default)
    {
        var res = await Include(_context.Set<T>()).FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        return Calculate(res);
    }

    /// <summary>
    /// Retrieves all entities that are not marked as deleted.
    /// </summary>
    /// <returns>An IQueryable of entities.</returns>
    public virtual IQueryable<T> GetAll()
    {
        return Include(_context.Set<T>());
    }

    public virtual IQueryable<T> GetAllBy(Guid parentEntityId)
    {
        return Include(_context.Set<T>());
    }

    public virtual async Task<T?> GetByIdFromScope(Guid id, Guid parentEntityId)
    {
        var res = await GetAllBy(parentEntityId).FirstOrDefaultAsync(x => x.Id == id);
        return Calculate(res);
    }

    /// <summary>
    /// Performs additional calculations or transformations on the entity.
    /// Override in derived classes to implement custom logic.
    /// </summary>
    /// <param name="model">The entity to process.</param>
    /// <returns>The processed entity.</returns>
    public virtual T? Calculate(T? model)
    {
        return model;
    }
}