using System.Linq.Expressions;
using Canteen.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Canteen.Data.Repositories;

public class Repository<TEntity>(ApplicationDbContext context)
    : IDisposable, IRepository<TEntity>, IAsyncDisposable
    where TEntity : class
{
    private static readonly char[] separator = [','];
    private readonly DbSet<TEntity> dbSet = context.Set<TEntity>();
    private bool _disposed;

    public async ValueTask DisposeAsync()
    {
        await DisposeAsync(true)
            .ConfigureAwait(false);
        GC.SuppressFinalize(this);
        await context.DisposeAsync()
            .ConfigureAwait(false);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
        context.Dispose();
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync(
        Expression<Func<TEntity, bool>>? filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        string includeProperties = "")
    {
        IQueryable<TEntity> query = dbSet;

        if (filter != null) query = query.Where(filter);

        query = includeProperties.Split(separator, StringSplitOptions.RemoveEmptyEntries)
            .Aggregate(query, (current, includeProperty) => current.Include(includeProperty));

        return orderBy != null ? await orderBy(query).ToListAsync() : await query.ToListAsync();
    }

    public async Task<TEntity> GetAsync(
        Expression<Func<TEntity, bool>>? filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        string includeProperties = "")
    {
        IQueryable<TEntity> query = dbSet;

        if (filter != null) query = query.Where(filter);

        query = includeProperties.Split(separator, StringSplitOptions.RemoveEmptyEntries)
            .Aggregate(query, (current, includeProperty) => current.Include(includeProperty));

        return orderBy != null ? await orderBy(query).FirstOrDefaultAsync() : await query.FirstOrDefaultAsync();
    }

    public TEntity GetById(object id)
    {
        return dbSet.Find(id);
    }

    public async Task<TEntity> GetByIdAsync(object id)
    {
        return await dbSet.FindAsync(id)
            .ConfigureAwait(false);
    }

    public void Add(TEntity entity)
    {
        dbSet.Add(entity);
    }

    public async Task<TEntity> AddAsync(TEntity entity)
    {
        await dbSet.AddAsync(entity)
            .ConfigureAwait(false);
        return entity;
    }

    public void Delete(object id)
    {
        var entityToDelete = dbSet.Find(id);
        Delete(entityToDelete);
    }

    public async Task<TEntity> DeleteAsync(object id)
    {
        var entityToDelete = await dbSet.FindAsync(id)
            .ConfigureAwait(false);
        await DeleteAsync(entityToDelete)
            .ConfigureAwait(false);
        return entityToDelete;
    }

    public void Delete(TEntity entityToDelete)
    {
        if (context.Entry(entityToDelete).State == EntityState.Detached) dbSet.Attach(entityToDelete);

        dbSet.Remove(entityToDelete);
    }

    public Task<TEntity> DeleteAsync(TEntity entityToDelete)
    {
        if (context.Entry(entityToDelete).State == EntityState.Detached) dbSet.Attach(entityToDelete);

        dbSet.Remove(entityToDelete);
        return Task.FromResult(entityToDelete);
    }

    public void Update(TEntity entityToUpdate)
    {
        dbSet.Attach(entityToUpdate);
        context.Entry(entityToUpdate).State = EntityState.Modified;
    }

    public Task<TEntity> UpdateAsync(TEntity entityToUpdate)
    {
        dbSet.Attach(entityToUpdate);
        context.Entry(entityToUpdate).State = EntityState.Modified;
        return Task.FromResult(entityToUpdate);
    }

    public async Task AddRangeAsync(IEnumerable<TEntity> entities)
    {
        await dbSet.AddRangeAsync(entities)
            .ConfigureAwait(false);
    }

    private void Dispose(bool isDisposed)
    {
        if (!_disposed)
            if (isDisposed)
                context.Dispose();

        _disposed = true;
    }

    private async ValueTask DisposeAsync(bool isDisposed)
    {
        if (!_disposed)
            if (isDisposed)
                await context.DisposeAsync()
                    .ConfigureAwait(false);

        _disposed = true;
    }
}