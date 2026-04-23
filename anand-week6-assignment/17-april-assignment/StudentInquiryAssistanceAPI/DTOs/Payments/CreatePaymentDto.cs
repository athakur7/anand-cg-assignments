using System.ComponentModel.DataAnnotations;

namespace StudentInquiryAssistanceAPI.DTOs.Payments;

public class CreatePaymentDto
{
    [Required]
    public int AdmissionId { get; set; }

    [Range(0.01, 999999)]
    public decimal Amount { get; set; }

    [Required]
    [StringLength(50)]
    public string PaymentMode { get; set; } = string.Empty;
}
