namespace StudentInquiryAssistanceAPI.DTOs.Payments;

public class BalanceDueDto
{
    public int AdmissionId { get; set; }

    public int CourseId { get; set; }

    public string CourseName { get; set; } = string.Empty;

    public decimal CourseFee { get; set; }

    public decimal TotalPaid { get; set; }

    public decimal BalanceAmount { get; set; }
}
