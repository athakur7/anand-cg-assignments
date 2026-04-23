using System.ComponentModel.DataAnnotations;

namespace StudentInquiryAssistanceAPI.DTOs.Courses;

public class UpsertCourseDto
{
    [Required]
    [StringLength(120)]
    public string CourseName { get; set; } = string.Empty;

    [Required]
    [StringLength(1000)]
    public string Description { get; set; } = string.Empty;

    [Required]
    [StringLength(120)]
    public string InstructorName { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Duration { get; set; } = string.Empty;

    [Range(0, 999999)]
    public decimal FeesAmount { get; set; }
}
