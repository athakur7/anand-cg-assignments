using System.ComponentModel.DataAnnotations;

namespace StudentInquiryAssistanceAPI.Models;

public class ErrorLog
{
    [Key]
    public int ErrorLogId { get; set; }

    [Required]
    [MaxLength(1000)]
    public string Message { get; set; } = string.Empty;

    public string? StackTrace { get; set; }

    [MaxLength(200)]
    public string? Path { get; set; }

    public int StatusCode { get; set; }

    public DateTime LoggedAt { get; set; } = DateTime.Now;
}
