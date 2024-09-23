namespace NET8.Demo.Shared;

public class PermissionDeniedException : BusinessException
{
    public PermissionDeniedException(string message, string details = null, Exception innerException = null)
        : base(ErrorCode.Forbidden, message, details, innerException) { }
}