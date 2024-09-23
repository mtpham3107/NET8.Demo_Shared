namespace NET8.Demo.Shared;

public class BusinessException : Exception
{
    public string Code { get; set; }

    public string Details { get; set; }

    public BusinessException(string code = null, string message = null, string details = null, Exception innerException = null) : base(message, innerException)
    {
        Code = code;
        Details = details;
    }

    public BusinessException WithData(string name, object value)
    {
        Data[name] = value;
        return this;
    }
}