namespace NET8.Demo.Shared;

public static class DateTimeExtension
{
    public static class DateTimeFormat
    {
        public const string ddMMyyyy = "dd/MM/yyyy";
        public const string ddMMyyyyHHmmss = "dd/MM/yyyy HH:mm:ss";
        public const string ddMMyyyyHHmm = "dd/MM/yyyy HH:mm";
        public const string yyyyMMddDash = "yyyy-MM-dd";
        public const string yyyyMMddSlash = "yyyy/MM/dd";
    }

    public static string ToString(this DateTime? dateTime, string format)
    {
        if (dateTime.HasValue)
        {
            return dateTime.Value.ToLocalTime().ToString(format);
        }
        return null;
    }

    public static string ToShortDateString(this DateTime? dateTime)
    {
        return dateTime.ToString(DateTimeFormat.ddMMyyyy);
    }

    public static string ToFullDateTimeString(this DateTime? dateTime)
    {
        return dateTime.ToString(DateTimeFormat.ddMMyyyyHHmmss);
    }

    public static string ToIsoDateString(this DateTime? dateTime)
    {
        return dateTime.ToString(DateTimeFormat.yyyyMMddDash);
    }
}
