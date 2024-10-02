using System.Linq.Expressions;

namespace Canteen.Data.Repositories.Interfaces;

public interface IRepository<TEntity> where TEntity : class
{
    Task<IEnumerable<TEntity>> GetAllAsync(
        Expression<Func<TEntity, bool>>? filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        string includeProperties = "");

    Task<TEntity> GetAsync(
        Expression<Func<TEntity, bool>>? filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        string includeProperties = "");

    TEntity GetById(object id);
    Task<TEntity> GetByIdAsync(object id);

    void Add(TEntity entity);
    Task<TEntity> AddAsync(TEntity entity);

    void Update(TEntity entity);
    Task<TEntity> UpdateAsync(TEntity entity);

    void Delete(object id);
    Task<TEntity> DeleteAsync(object id);

    void Delete(TEntity entity);
    Task<TEntity> DeleteAsync(TEntity entity);

    Task AddRangeAsync(IEnumerable<TEntity> entities);
}