using System.ComponentModel.DataAnnotations;

namespace StudentInquiryAssistanceAPI.Models;

public class Payment
{
    [Key]
    public int PaymentId { get; set; }

    public DateTime PaymentDate { get; set; } = DateTime.Now;

    [Range(0.01, 999999)]
    public decimal Amount { get; set; }

    [Required]
    [MaxLength(50)]
    public string PaymentMode { get; set; } = string.Empty;

    public int StudentId { get; set; }

    public int AdmissionId { get; set; }

    public int CourseId { get; set; }

    public Student Student { get; set; } = null!;

    public Admission Admission { get; set; } = null!;

    public Course Course { get; set; } = null!;
}
