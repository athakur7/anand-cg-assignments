using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentInquiryAssistanceAPI.Constants;
using StudentInquiryAssistanceAPI.Data;
using StudentInquiryAssistanceAPI.DTOs.Courses;
using StudentInquiryAssistanceAPI.Extensions;
using StudentInquiryAssistanceAPI.Models;

namespace StudentInquiryAssistanceAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CourseController(ApplicationDbContext context) : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAllCourses([FromQuery] string? search)
    {
        var query = context.Courses.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchTerm = search.Trim();
            query = query.Where(c =>
                c.CourseName.Contains(searchTerm) ||
                c.Description.Contains(searchTerm) ||
                c.InstructorName.Contains(searchTerm));
        }

        var courses = await query
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

    [HttpGet("{courseId:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(int courseId)
    {
        var course = await context.Courses.AsNoTracking().FirstOrDefaultAsync(c => c.CourseId == courseId);
        if (course is null)
        {
            return NotFound(new { message = "Course not found." });
        }

        return Ok(MapCourse(course));
    }

    [HttpPost]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<IActionResult> SaveCourseByAdmin(UpsertCourseDto request)
    {
        var course = new Course
        {
            CourseName = request.CourseName.Trim(),
            Description = request.Description.Trim(),
            InstructorName = request.InstructorName.Trim(),
            Duration = request.Duration.Trim(),
            FeesAmount = request.FeesAmount,
            CreatedByUserId = User.GetRequiredUserId()
        };

        context.Courses.Add(course);
        await context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { courseId = course.CourseId }, MapCourse(course));
    }

    [HttpPut("{courseId:int}")]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<IActionResult> UpdateCourseByAdmin(int courseId, UpsertCourseDto request)
    {
        var course = await context.Courses.FirstOrDefaultAsync(c => c.CourseId == courseId);
        if (course is null)
        {
            return NotFound(new { message = "Course not found." });
        }

        course.CourseName = request.CourseName.Trim();
        course.Description = request.Description.Trim();
        course.InstructorName = request.InstructorName.Trim();
        course.Duration = request.Duration.Trim();
        course.FeesAmount = request.FeesAmount;

        await context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{courseId:int}")]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<IActionResult> DeleteCourseByAdmin(int courseId)
    {
        var course = await context.Courses.FirstOrDefaultAsync(c => c.CourseId == courseId);
        if (course is null)
        {
            return NotFound(new { message = "Course not found." });
        }

        var hasDependencies = await context.Enquiries.AnyAsync(e => e.CourseId == courseId)
            || await context.Admissions.AnyAsync(a => a.CourseId == courseId)
            || await context.Payments.AnyAsync(p => p.CourseId == courseId);

        if (hasDependencies)
        {
            return BadRequest(new { message = "Course cannot be deleted because related enquiries, admissions, or payments already exist." });
        }

        context.Courses.Remove(course);
        await context.SaveChangesAsync();
        return NoContent();
    }

    private static CourseDto MapCourse(Course course)
    {
        return new CourseDto
        {
            CourseId = course.CourseId,
            CourseName = course.CourseName,
            Description = course.Description,
            InstructorName = course.InstructorName,
            Duration = course.Duration,
            FeesAmount = course.FeesAmount
        };
    }
}
