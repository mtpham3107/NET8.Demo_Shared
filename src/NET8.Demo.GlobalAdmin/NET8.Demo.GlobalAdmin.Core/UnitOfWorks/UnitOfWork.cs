using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using NET8.Demo.GlobalAdmin.Core.DbContexts;
using NET8.Demo.GlobalAdmin.Domain;
using NET8.Demo.GlobalAdmin.Domain.IRepositories;
using NET8.Demo.GlobalAdmin.Domain.IUnitOfWorks;

namespace NET8.Demo.GlobalAdmin.Core.UnitOfWorks;

public class UnitOfWork : IUnitOfWork
{
    protected readonly GlobalAdminDbContext _context;
    private readonly IServiceProvider _serviceProvider;
    private IDbContextTransaction _transaction;
    private bool _disposed = false;

    public UnitOfWork(GlobalAdminDbContext context, IServiceProvider serviceProvider)
    {
        _context = context;
        _serviceProvider = serviceProvider;
    }

    public IRepository<TEntity> Repository<TEntity>() where TEntity : EntityBase
    {
        return _serviceProvider.GetRequiredService<IRepository<TEntity>>();
    }

    public int SaveChanges()
    {
        return _context.SaveChanges();
    }

    public async ValueTask<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void BeginTransaction()
    {
        if (_transaction == null)
        {
            _transaction = _context.Database.BeginTransaction();
        }
    }

    public async ValueTask BeginTransactionAsync()
    {
        if (_transaction == null)
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }
    }

    public void CommitTransaction()
    {
        _transaction?.Commit();
        _transaction = null;
    }

    public void RollBackTransaction()
    {
        _transaction?.Rollback();
        _transaction = null;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
        _disposed = true;
    }
}
