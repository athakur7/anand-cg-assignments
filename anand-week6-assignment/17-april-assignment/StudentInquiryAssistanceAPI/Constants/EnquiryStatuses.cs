namespace StudentInquiryAssistanceAPI.Constants;

public static class EnquiryStatuses
{
    public const string Pending = "Pending";
    public const string Replied = "Replied";
    public const string Closed = "Closed";

    public static readonly HashSet<string> Allowed = new(StringComparer.OrdinalIgnoreCase)
    {
        Pending,
        Replied,
        Closed
    };
}
