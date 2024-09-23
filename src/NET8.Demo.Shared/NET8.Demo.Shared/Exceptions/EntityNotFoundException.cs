namespace NET8.Demo.Shared;

public class EntityNotFoundException : BusinessException
{
    public EntityNotFoundException(string message, string details = null, Exception innerException = null)
        : base(ErrorCode.NoContent, message, details, innerException) { }
}