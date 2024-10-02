using Canteen.Data.Repositories.Interfaces;

namespace Canteen.Data.UnitOfWork.Interfaces;

public interface IUnitOfWork
{
    IRepository<T> Repository<T>() where T : class;
    Task<int> SaveChangesAsync();
}