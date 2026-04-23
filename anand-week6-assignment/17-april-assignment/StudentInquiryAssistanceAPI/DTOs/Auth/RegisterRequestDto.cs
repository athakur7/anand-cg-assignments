using System.ComponentModel.DataAnnotations;

namespace StudentInquiryAssistanceAPI.DTOs.Auth;

public class RegisterRequestDto
{
    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d).{6,}$", ErrorMessage = "Password must be at least 6 characters and contain letters and numbers.")]
    public string Password { get; set; } = string.Empty;

    [Required]
    [RegularExpression(@"^\d{10}$", ErrorMessage = "Invalid mobile number format.")]
    public string MobileNumber { get; set; } = string.Empty;

    [Required]
    public string UserRole { get; set; } = string.Empty;

    [StringLength(120)]
    public string? StudentName { get; set; }
}
