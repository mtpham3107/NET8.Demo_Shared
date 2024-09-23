namespace NET8.Demo.TemplateService.Domain.IRepositories;

public interface IDataSyncRedisRepository<TEntity> where TEntity : EntityBase
{
    ValueTask<TEntity> GetDataSyncToRedisAsync(Guid id);

    ValueTask<IEnumerable<TEntity>> GetDataSyncToRedisAsync(IEnumerable<Guid> ids = null);
}
