using System.ComponentModel.DataAnnotations;
using StudentInquiryAssistanceAPI.Constants;

namespace StudentInquiryAssistanceAPI.Models;

public class Admission
{
    [Key]
    public int AdmissionId { get; set; }

    public DateTime AdmissionDate { get; set; } = DateTime.Now;

    [Required]
    [MaxLength(30)]
    public string Status { get; set; } = AdmissionStatuses.Applied;

    public int StudentId { get; set; }

    public int CourseId { get; set; }

    public Student Student { get; set; } = null!;

    public Course Course { get; set; } = null!;

    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
