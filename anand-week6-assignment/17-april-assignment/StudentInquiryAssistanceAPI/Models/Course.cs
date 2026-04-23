using System.ComponentModel.DataAnnotations;

namespace StudentInquiryAssistanceAPI.Models;

public class Course
{
    [Key]
    public int CourseId { get; set; }

    [Required]
    [MaxLength(120)]
    public string CourseName { get; set; } = string.Empty;

    [Required]
    [MaxLength(1000)]
    public string Description { get; set; } = string.Empty;

    [Required]
    [MaxLength(120)]
    public string InstructorName { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Duration { get; set; } = string.Empty;

    [Range(0, 999999)]
    public decimal FeesAmount { get; set; }

    public long? CreatedByUserId { get; set; }

    public User? CreatedByUser { get; set; }

    public ICollection<Enquiry> Enquiries { get; set; } = new List<Enquiry>();

    public ICollection<Admission> Admissions { get; set; } = new List<Admission>();

    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
