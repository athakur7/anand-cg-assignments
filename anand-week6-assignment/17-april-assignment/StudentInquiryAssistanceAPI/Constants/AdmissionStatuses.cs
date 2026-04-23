namespace StudentInquiryAssistanceAPI.Constants;

public static class AdmissionStatuses
{
    public const string Applied = "Applied";
    public const string Approved = "Approved";
    public const string Rejected = "Rejected";
    public const string Closed = "Closed";

    public static readonly HashSet<string> Allowed = new(StringComparer.OrdinalIgnoreCase)
    {
        Applied,
        Approved,
        Rejected,
        Closed
    };
}
