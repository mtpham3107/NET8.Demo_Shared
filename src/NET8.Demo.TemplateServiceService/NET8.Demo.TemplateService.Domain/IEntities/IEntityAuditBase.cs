namespace NET8.Demo.TemplateService.Domain.IEntities;

public interface IEntityAuditBase<T> : IEntityPrimaryKeyBase<T>, IEntityTrackingInfo, IEntityDeletionInfo
{
}
