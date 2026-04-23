using System.ComponentModel.DataAnnotations;

namespace StudentInquiryAssistanceAPI.Models;

public class Student
{
    [Key]
    public int StudentId { get; set; }

    [Required]
    [MaxLength(120)]
    public string StudentName { get; set; } = string.Empty;

    [Required]
    [MaxLength(120)]
    [EmailAddress]
    public string StudentEmailId { get; set; } = string.Empty;

    public long UserId { get; set; }

    public User User { get; set; } = null!;

    public ICollection<Enquiry> Enquiries { get; set; } = new List<Enquiry>();

    public ICollection<Admission> Admissions { get; set; } = new List<Admission>();

    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
