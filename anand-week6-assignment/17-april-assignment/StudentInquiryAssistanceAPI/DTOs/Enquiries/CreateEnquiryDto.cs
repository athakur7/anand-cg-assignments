using System.ComponentModel.DataAnnotations;

namespace StudentInquiryAssistanceAPI.DTOs.Enquiries;

public class CreateEnquiryDto
{
    [Required]
    public int CourseId { get; set; }

    [Required]
    [StringLength(150)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(1000)]
    public string Description { get; set; } = string.Empty;

    [Required]
    [StringLength(80)]
    public string EnquiryType { get; set; } = string.Empty;
}
