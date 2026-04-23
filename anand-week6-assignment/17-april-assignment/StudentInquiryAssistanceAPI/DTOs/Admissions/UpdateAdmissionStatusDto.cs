using System.ComponentModel.DataAnnotations;

namespace StudentInquiryAssistanceAPI.DTOs.Admissions;

public class UpdateAdmissionStatusDto
{
    [Required]
    public string Status { get; set; } = string.Empty;
}
