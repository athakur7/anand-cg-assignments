using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentInquiryAssistanceAPI.Constants;
using StudentInquiryAssistanceAPI.Data;
using StudentInquiryAssistanceAPI.DTOs.Enquiries;
using StudentInquiryAssistanceAPI.Extensions;
using StudentInquiryAssistanceAPI.Models;

namespace StudentInquiryAssistanceAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EnquiryController(ApplicationDbContext context) : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.OfficeStaff}")]
    public async Task<IActionResult> GetAllEnquiriesByAdmin()
    {
        var enquiries = await context.Enquiries
            .AsNoTracking()
            .Include(e => e.Student)
            .ThenInclude(s => s.User)
            .Include(e => e.Course)
            .OrderByDescending(e => e.EnquiryDate)
            .ToListAsync();

        return Ok(enquiries.Select(MapEnquiry));
    }

    [HttpGet("{enquiryId:int}")]
    [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.OfficeStaff},{AppRoles.Student}")]
    public async Task<IActionResult> GetEnquiryByAdmin(int enquiryId)
    {
        var enquiry = await context.Enquiries
            .AsNoTracking()
            .Include(e => e.Student)
            .ThenInclude(s => s.User)
            .Include(e => e.Course)
            .FirstOrDefaultAsync(e => e.EnquiryId == enquiryId);

        if (enquiry is null)
        {
            return NotFound(new { message = "Enquiry not found." });
        }

        if (User.IsInRole(AppRoles.Student) && enquiry.Student.UserId != User.GetRequiredUserId())
        {
            return Forbid();
        }

        return Ok(MapEnquiry(enquiry));
    }

    [HttpGet("UserId/{userId:long}")]
    [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.OfficeStaff},{AppRoles.Student}")]
    public async Task<IActionResult> GetEnquiriesByUser(long userId)
    {
        if (User.IsInRole(AppRoles.Student) && userId != User.GetRequiredUserId())
        {
            return Forbid();
        }

        var enquiries = await context.Enquiries
            .AsNoTracking()
            .Include(e => e.Student)
            .ThenInclude(s => s.User)
            .Include(e => e.Course)
            .Where(e => e.Student.UserId == userId)
            .OrderByDescending(e => e.EnquiryDate)
            .ToListAsync();

        return Ok(enquiries.Select(MapEnquiry));
    }

    [HttpGet("my")]
    [Authorize(Roles = AppRoles.Student)]
    public async Task<IActionResult> GetMyEnquiries()
    {
        var userId = User.GetRequiredUserId();
        return await GetEnquiriesByUser(userId);
    }

    [HttpPost]
    [Authorize(Roles = AppRoles.Student)]
    public async Task<IActionResult> CreateEnquiry(CreateEnquiryDto request)
    {
        var studentId = User.GetRequiredStudentId();
        var courseExists = await context.Courses.AnyAsync(c => c.CourseId == request.CourseId);
        if (!courseExists)
        {
            return NotFound(new { message = "Course not found." });
        }

        var today = DateTime.Today;
        var tomorrow = today.AddDays(1);

        var dailyCount = await context.Enquiries.CountAsync(e =>
            e.StudentId == studentId &&
            e.EnquiryDate >= today &&
            e.EnquiryDate < tomorrow);

        if (dailyCount >= 5)
        {
            return BadRequest(new { message = "Daily enquiry limit reached. A student can submit only 5 enquiries per day." });
        }

        var enquiry = new Enquiry
        {
            StudentId = studentId,
            CourseId = request.CourseId,
            Title = request.Title.Trim(),
            Description = request.Description.Trim(),
            EnquiryType = request.EnquiryType.Trim(),
            Status = EnquiryStatuses.Pending,
            EnquiryDate = DateTime.Now
        };

        context.Enquiries.Add(enquiry);
        await context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetEnquiryByAdmin), new { enquiryId = enquiry.EnquiryId }, new
        {
            message = "Enquiry submitted successfully.",
            enquiry.EnquiryId
        });
    }

    [HttpPatch("{enquiryId:int}/status")]
    [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.OfficeStaff}")]
    public async Task<IActionResult> UpdateEnquiryStatus(int enquiryId, UpdateEnquiryStatusDto request)
    {
        if (!EnquiryStatuses.Allowed.Contains(request.Status))
        {
            return BadRequest(new { message = "Invalid enquiry status." });
        }

        var enquiry = await context.Enquiries.FirstOrDefaultAsync(e => e.EnquiryId == enquiryId);
        if (enquiry is null)
        {
            return NotFound(new { message = "Enquiry not found." });
        }

        enquiry.Status = EnquiryStatuses.Allowed.First(status =>
            status.Equals(request.Status, StringComparison.OrdinalIgnoreCase));
        enquiry.ResponseMessage = string.IsNullOrWhiteSpace(request.ResponseMessage)
            ? enquiry.ResponseMessage
            : request.ResponseMessage.Trim();
        enquiry.RespondedOn = DateTime.Now;
        enquiry.RespondedByUserId = User.GetRequiredUserId();

        await context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{enquiryId:int}")]
    [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Student}")]
    public async Task<IActionResult> DeleteEnquiry(int enquiryId)
    {
        var enquiry = await context.Enquiries
            .Include(e => e.Student)
            .ThenInclude(s => s.User)
            .FirstOrDefaultAsync(e => e.EnquiryId == enquiryId);

        if (enquiry is null)
        {
            return NotFound(new { message = "Enquiry not found." });
        }

        if (User.IsInRole(AppRoles.Student) && enquiry.Student.UserId != User.GetRequiredUserId())
        {
            return Forbid();
        }

        context.Enquiries.Remove(enquiry);
        await context.SaveChangesAsync();
        return NoContent();
    }

    private static EnquiryDto MapEnquiry(Enquiry enquiry)
    {
        return new EnquiryDto
        {
            EnquiryId = enquiry.EnquiryId,
            EnquiryDate = enquiry.EnquiryDate,
            Title = enquiry.Title,
            Description = enquiry.Description,
            EnquiryType = enquiry.EnquiryType,
            Status = enquiry.Status,
            ResponseMessage = enquiry.ResponseMessage,
            RespondedOn = enquiry.RespondedOn,
            StudentId = enquiry.StudentId,
            StudentName = enquiry.Student.StudentName,
            UserId = enquiry.Student.UserId,
            CourseId = enquiry.CourseId,
            CourseName = enquiry.Course.CourseName
        };
    }
}
