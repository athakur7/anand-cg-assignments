using System.ComponentModel.DataAnnotations;
using StudentInquiryAssistanceAPI.Constants;

namespace StudentInquiryAssistanceAPI.Models;

public class Enquiry
{
    [Key]
    public int EnquiryId { get; set; }

    public DateTime EnquiryDate { get; set; } = DateTime.Now;

    [Required]
    [MaxLength(150)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(1000)]
    public string Description { get; set; } = string.Empty;

    [Required]
    [MaxLength(80)]
    public string EnquiryType { get; set; } = string.Empty;

    [Required]
    [MaxLength(30)]
    public string Status { get; set; } = EnquiryStatuses.Pending;

    [MaxLength(1000)]
    public string? ResponseMessage { get; set; }

    public DateTime? RespondedOn { get; set; }

    public long? RespondedByUserId { get; set; }

    public int StudentId { get; set; }

    public int CourseId { get; set; }

    public Student Student { get; set; } = null!;

    public Course Course { get; set; } = null!;

    public User? RespondedByUser { get; set; }
}
