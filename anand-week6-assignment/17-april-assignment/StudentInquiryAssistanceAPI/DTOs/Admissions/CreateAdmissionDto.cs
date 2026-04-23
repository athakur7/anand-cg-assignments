using System.ComponentModel.DataAnnotations;

namespace StudentInquiryAssistanceAPI.DTOs.Admissions;

public class CreateAdmissionDto
{
    [Required]
    public int CourseId { get; set; }
}
