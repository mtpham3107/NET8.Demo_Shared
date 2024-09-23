using NET8.Demo.GlobalAdmin.Domain.IRepositories;

namespace NET8.Demo.GlobalAdmin.Domain.IUnitOfWorks;

public interface IUnitOfWork : IDisposable
{
    IRepository<TEntity> Repository<TEntity>() where TEntity : EntityBase;

    int SaveChanges();

    ValueTask<int> SaveChangesAsync();

    void BeginTransaction();

    ValueTask BeginTransactionAsync();

    void CommitTransaction();

    void RollBackTransaction();
}
