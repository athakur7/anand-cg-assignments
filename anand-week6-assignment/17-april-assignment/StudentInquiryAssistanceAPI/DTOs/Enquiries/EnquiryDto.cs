namespace StudentInquiryAssistanceAPI.DTOs.Enquiries;

public class EnquiryDto
{
    public int EnquiryId { get; set; }

    public DateTime EnquiryDate { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string EnquiryType { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public string? ResponseMessage { get; set; }

    public DateTime? RespondedOn { get; set; }

    public int StudentId { get; set; }

    public string StudentName { get; set; } = string.Empty;

    public long UserId { get; set; }

    public int CourseId { get; set; }

    public string CourseName { get; set; } = string.Empty;
}
