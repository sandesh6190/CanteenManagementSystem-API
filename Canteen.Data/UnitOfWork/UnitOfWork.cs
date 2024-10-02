using Canteen.Data.Repositories;
using Canteen.Data.Repositories.Interfaces;
using Canteen.Data.UnitOfWork.Interfaces;

namespace Canteen.Data.UnitOfWork;

public class UnitOfWork(ApplicationDbContext context) : IUnitOfWork, IDisposable, IAsyncDisposable
{
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

    public IRepository<T> Repository<T>() where T : class
    {
        return new Repository<T>(context);
    }

    public Task<int> SaveChangesAsync()
    {
        return context.SaveChangesAsync();
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