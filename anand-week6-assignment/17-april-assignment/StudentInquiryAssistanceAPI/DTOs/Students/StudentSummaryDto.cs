namespace StudentInquiryAssistanceAPI.DTOs.Students;

public class StudentSummaryDto
{
    public int StudentId { get; set; }

    public string StudentName { get; set; } = string.Empty;

    public string StudentEmailId { get; set; } = string.Empty;

    public long UserId { get; set; }

    public string Username { get; set; } = string.Empty;

    public string MobileNumber { get; set; } = string.Empty;
}
