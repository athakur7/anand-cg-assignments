namespace StudentInquiryAssistanceAPI.DTOs.Auth;

public class AuthResponseDto
{
    public long UserId { get; set; }

    public int? StudentId { get; set; }

    public string Username { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string UserRole { get; set; } = string.Empty;

    public string Token { get; set; } = string.Empty;
}
