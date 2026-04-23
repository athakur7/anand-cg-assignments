using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentInquiryAssistanceAPI.Constants;
using StudentInquiryAssistanceAPI.Data;
using StudentInquiryAssistanceAPI.DTOs.Auth;
using StudentInquiryAssistanceAPI.Models;
using StudentInquiryAssistanceAPI.Services;

namespace StudentInquiryAssistanceAPI.Controllers;

[ApiController]
[Route("auth")]
public class AuthController(
    ApplicationDbContext context,
    IPasswordHasher<User> passwordHasher,
    ITokenService tokenService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequestDto request)
    {
        var normalizedRole = NormalizeRole(request.UserRole);
        if (normalizedRole is null)
        {
            return BadRequest(new { message = "Invalid user role. Allowed roles are Admin, Student, and OfficeStaff." });
        }

        var email = request.Email.Trim().ToLowerInvariant();
        var username = request.Username.Trim();

        var emailExists = await context.Users.AnyAsync(u => u.Email == email);
        if (emailExists)
        {
            return BadRequest(new { message = "Email is already registered." });
        }

        var usernameExists = await context.Users.AnyAsync(u => u.Username == username);
        if (usernameExists)
        {
            return BadRequest(new { message = "Username is already registered." });
        }

        var user = new User
        {
            Email = email,
            Username = username,
            MobileNumber = request.MobileNumber.Trim(),
            UserRole = normalizedRole
        };
        user.PasswordHash = passwordHasher.HashPassword(user, request.Password);

        context.Users.Add(user);
        await context.SaveChangesAsync();

        int? studentId = null;
        if (normalizedRole == AppRoles.Student)
        {
            var student = new Student
            {
                StudentName = string.IsNullOrWhiteSpace(request.StudentName) ? username : request.StudentName.Trim(),
                StudentEmailId = email,
                UserId = user.UserId
            };

            context.Students.Add(student);
            await context.SaveChangesAsync();
            studentId = student.StudentId;
        }

        var response = new AuthResponseDto
        {
            UserId = user.UserId,
            StudentId = studentId,
            Username = user.Username,
            Email = user.Email,
            UserRole = user.UserRole,
            Token = tokenService.CreateToken(user, studentId)
        };

        return CreatedAtAction(nameof(Register), new { id = user.UserId }, response);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequestDto request)
    {
        var loginValue = request.Email.Trim();
        var normalizedEmail = loginValue.ToLowerInvariant();

        var user = await context.Users
            .Include(u => u.Student)
            .FirstOrDefaultAsync(u => u.Email == normalizedEmail || u.Username == loginValue);

        if (user is null)
        {
            return Unauthorized(new { message = "Invalid email or password." });
        }

        var verification = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (verification == PasswordVerificationResult.Failed)
        {
            return Unauthorized(new { message = "Invalid email or password." });
        }

        var response = new AuthResponseDto
        {
            UserId = user.UserId,
            StudentId = user.Student?.StudentId,
            Username = user.Username,
            Email = user.Email,
            UserRole = user.UserRole,
            Token = tokenService.CreateToken(user, user.Student?.StudentId)
        };

        return Ok(response);
    }

    private static string? NormalizeRole(string role)
    {
        return AppRoles.All.FirstOrDefault(existingRole =>
            existingRole.Equals(role.Trim(), StringComparison.OrdinalIgnoreCase));
    }
}
