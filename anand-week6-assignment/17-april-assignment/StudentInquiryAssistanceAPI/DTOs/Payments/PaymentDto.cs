namespace StudentInquiryAssistanceAPI.DTOs.Payments;

public class PaymentDto
{
    public int PaymentId { get; set; }

    public DateTime PaymentDate { get; set; }

    public decimal Amount { get; set; }

    public string PaymentMode { get; set; } = string.Empty;

    public int StudentId { get; set; }

    public string StudentName { get; set; } = string.Empty;

    public long UserId { get; set; }

    public int AdmissionId { get; set; }

    public int CourseId { get; set; }

    public string CourseName { get; set; } = string.Empty;
}
