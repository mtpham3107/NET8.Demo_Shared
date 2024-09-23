namespace NET8.Demo.GlobalAdmin.Domain.IEntities;

public interface IEntityAuditBase<T> : IEntityPrimaryKeyBase<T>, IEntityTrackingInfo, IEntityDeletionInfo
{
}
