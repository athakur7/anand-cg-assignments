namespace StudentInquiryAssistanceAPI.DTOs.Admissions;

public class AdmissionDto
{
    public int AdmissionId { get; set; }

    public DateTime AdmissionDate { get; set; }

    public string Status { get; set; } = string.Empty;

    public int StudentId { get; set; }

    public string StudentName { get; set; } = string.Empty;

    public long UserId { get; set; }

    public int CourseId { get; set; }

    public string CourseName { get; set; } = string.Empty;

    public decimal TotalCourseFee { get; set; }

    public decimal TotalPaid { get; set; }

    public decimal BalanceAmount { get; set; }
}
