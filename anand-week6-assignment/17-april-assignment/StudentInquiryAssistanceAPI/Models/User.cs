using System.ComponentModel.DataAnnotations;

namespace StudentInquiryAssistanceAPI.Models;

public class User
{
    [Key]
    public long UserId { get; set; }

    [Required]
    [MaxLength(120)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [MaxLength(256)]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    [MaxLength(10)]
    public string MobileNumber { get; set; } = string.Empty;

    [Required]
    [MaxLength(30)]
    public string UserRole { get; set; } = string.Empty;

    public Student? Student { get; set; }

    public ICollection<Course> Courses { get; set; } = new List<Course>();

    public ICollection<Enquiry> RepliedEnquiries { get; set; } = new List<Enquiry>();
}
