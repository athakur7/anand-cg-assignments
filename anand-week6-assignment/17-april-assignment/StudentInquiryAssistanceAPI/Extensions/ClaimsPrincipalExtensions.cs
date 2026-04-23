using System.Security.Claims;
using StudentInquiryAssistanceAPI.Exceptions;

namespace StudentInquiryAssistanceAPI.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static long GetRequiredUserId(this ClaimsPrincipal user)
    {
        var value = user.FindFirstValue(ClaimTypes.NameIdentifier);
        return long.TryParse(value, out var userId)
            ? userId
            : throw new BadRequestException("Invalid authenticated user.");
    }

    public static int GetRequiredStudentId(this ClaimsPrincipal user)
    {
        var value = user.FindFirstValue("studentId");
        return int.TryParse(value, out var studentId)
            ? studentId
            : throw new BadRequestException("Student profile was not found for the authenticated user.");
    }
}
