using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using StudentInquiryAssistanceAPI.Models;

namespace StudentInquiryAssistanceAPI.Services;

public class TokenService(IConfiguration configuration) : ITokenService
{
    public string CreateToken(User user, int? studentId)
    {
        var secret = configuration["JWT:Secret"] ?? throw new InvalidOperationException("JWT secret is missing.");
        var issuer = configuration["JWT:ValidIssuer"] ?? throw new InvalidOperationException("JWT issuer is missing.");
        var audience = configuration["JWT:ValidAudience"] ?? throw new InvalidOperationException("JWT audience is missing.");

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.UserRole)
        };

        if (studentId.HasValue)
        {
            claims.Add(new Claim("studentId", studentId.Value.ToString()));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
