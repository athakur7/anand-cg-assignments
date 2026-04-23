using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentInquiryAssistanceAPI.Constants;
using StudentInquiryAssistanceAPI.Data;
using StudentInquiryAssistanceAPI.DTOs.Courses;
using StudentInquiryAssistanceAPI.DTOs.Students;
using StudentInquiryAssistanceAPI.Extensions;

namespace StudentInquiryAssistanceAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StudentController(ApplicationDbContext context) : ControllerBase
{
    [HttpGet("course")]
    [AllowAnonymous]
    public async Task<IActionResult> GetStudentCourses()
    {
        var courses = await context.Courses
            .AsNoTracking()
            .OrderBy(c => c.CourseName)
            .Select(c => new CourseDto
            {
                CourseId = c.CourseId,
                CourseName = c.CourseName,
                Description = c.Description,
                InstructorName = c.InstructorName,
                Duration = c.Duration,
                FeesAmount = c.FeesAmount
            })
            .ToListAsync();

        return Ok(courses);
    }

    [HttpGet]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<IActionResult> GetAllStudents()
    {
        var students = await context.Students
            .AsNoTracking()
            .Include(s => s.User)
            .OrderBy(s => s.StudentName)
            .Select(s => new StudentSummaryDto
            {
                StudentId = s.StudentId,
                StudentName = s.StudentName,
                StudentEmailId = s.StudentEmailId,
                UserId = s.UserId,
                Username = s.User.Username,
                MobileNumber = s.User.MobileNumber
            })
            .ToListAsync();

        return Ok(students);
    }

    [HttpGet("me")]
    [Authorize(Roles = AppRoles.Student)]
    public async Task<IActionResult> GetMyProfile()
    {
        var userId = User.GetRequiredUserId();

        var student = await context.Students
            .AsNoTracking()
            .Include(s => s.User)
            .FirstOrDefaultAsync(s => s.UserId == userId);

        if (student is null)
        {
            return NotFound(new { message = "Student profile not found." });
        }

        return Ok(new StudentSummaryDto
        {
            StudentId = student.StudentId,
            StudentName = student.StudentName,
            StudentEmailId = student.StudentEmailId,
            UserId = student.UserId,
            Username = student.User.Username,
            MobileNumber = student.User.MobileNumber
        });
    }
}
