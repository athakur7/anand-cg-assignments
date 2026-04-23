using StudentInquiryAssistanceAPI.Models;

namespace StudentInquiryAssistanceAPI.Services;

public interface ITokenService
{
    string CreateToken(User user, int? studentId);
}
