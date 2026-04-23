using System.ComponentModel.DataAnnotations;

namespace StudentInquiryAssistanceAPI.DTOs.Enquiries;

public class UpdateEnquiryStatusDto
{
    [Required]
    public string Status { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? ResponseMessage { get; set; }
}
