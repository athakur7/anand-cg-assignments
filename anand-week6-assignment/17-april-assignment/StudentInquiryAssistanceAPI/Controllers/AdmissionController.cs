using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentInquiryAssistanceAPI.Constants;
using StudentInquiryAssistanceAPI.Data;
using StudentInquiryAssistanceAPI.DTOs.Admissions;
using StudentInquiryAssistanceAPI.Extensions;
using StudentInquiryAssistanceAPI.Models;

namespace StudentInquiryAssistanceAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdmissionController(ApplicationDbContext context) : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.OfficeStaff}")]
    public async Task<IActionResult> GetAllAdmissions()
    {
        var admissions = await context.Admissions
            .AsNoTracking()
            .Include(a => a.Student)
            .ThenInclude(s => s.User)
            .Include(a => a.Course)
            .Include(a => a.Payments)
            .OrderByDescending(a => a.AdmissionDate)
            .ToListAsync();

        return Ok(admissions.Select(MapAdmission));
    }

    [HttpGet("{admissionId:int}")]
    [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.OfficeStaff},{AppRoles.Student}")]
    public async Task<IActionResult> GetById(int admissionId)
    {
        var admission = await context.Admissions
            .AsNoTracking()
            .Include(a => a.Student)
            .ThenInclude(s => s.User)
            .Include(a => a.Course)
            .Include(a => a.Payments)
            .FirstOrDefaultAsync(a => a.AdmissionId == admissionId);

        if (admission is null)
        {
            return NotFound(new { message = "Admission not found." });
        }

        if (User.IsInRole(AppRoles.Student) && admission.Student.UserId != User.GetRequiredUserId())
        {
            return Forbid();
        }

        return Ok(MapAdmission(admission));
    }

    [HttpGet("user/{userId:long}")]
    [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.OfficeStaff},{AppRoles.Student}")]
    public async Task<IActionResult> GetAdmissionsByUser(long userId)
    {
        if (User.IsInRole(AppRoles.Student) && userId != User.GetRequiredUserId())
        {
            return Forbid();
        }

        var admissions = await context.Admissions
            .AsNoTracking()
            .Include(a => a.Student)
            .ThenInclude(s => s.User)
            .Include(a => a.Course)
            .Include(a => a.Payments)
            .Where(a => a.Student.UserId == userId)
            .OrderByDescending(a => a.AdmissionDate)
            .ToListAsync();

        return Ok(admissions.Select(MapAdmission));
    }

    [HttpPost]
    [Authorize(Roles = AppRoles.Student)]
    public async Task<IActionResult> CreateAdmission(CreateAdmissionDto request)
    {
        var studentId = User.GetRequiredStudentId();

        var course = await context.Courses.FirstOrDefaultAsync(c => c.CourseId == request.CourseId);
        if (course is null)
        {
            return NotFound(new { message = "Course not found." });
        }

        var exists = await context.Admissions.AnyAsync(a => a.StudentId == studentId && a.CourseId == request.CourseId);
        if (exists)
        {
            return BadRequest(new { message = "Admission already exists for this student and course." });
        }

        var admission = new Admission
        {
            StudentId = studentId,
            CourseId = request.CourseId,
            Status = AdmissionStatuses.Applied,
            AdmissionDate = DateTime.Now
        };

        context.Admissions.Add(admission);
        await context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { admissionId = admission.AdmissionId }, new
        {
            message = "Admission created successfully.",
            admission.AdmissionId
        });
    }

    [HttpPut("{admissionId:int}/status")]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<IActionResult> UpdateAdmissionStatus(int admissionId, UpdateAdmissionStatusDto request)
    {
        if (!AdmissionStatuses.Allowed.Contains(request.Status))
        {
            return BadRequest(new { message = "Invalid admission status." });
        }

        var admission = await context.Admissions.FirstOrDefaultAsync(a => a.AdmissionId == admissionId);
        if (admission is null)
        {
            return NotFound(new { message = "Admission not found." });
        }

        admission.Status = AdmissionStatuses.Allowed.First(status =>
            status.Equals(request.Status, StringComparison.OrdinalIgnoreCase));

        await context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{admissionId:int}")]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<IActionResult> DeleteAdmission(int admissionId)
    {
        var admission = await context.Admissions.FirstOrDefaultAsync(a => a.AdmissionId == admissionId);
        if (admission is null)
        {
            return NotFound(new { message = "Admission not found." });
        }

        var hasPayments = await context.Payments.AnyAsync(p => p.AdmissionId == admissionId);
        if (hasPayments)
        {
            return BadRequest(new { message = "Admission cannot be deleted because payment records already exist." });
        }

        context.Admissions.Remove(admission);
        await context.SaveChangesAsync();
        return NoContent();
    }

    private static AdmissionDto MapAdmission(Admission admission)
    {
        var totalPaid = admission.Payments.Sum(p => p.Amount);

        return new AdmissionDto
        {
            AdmissionId = admission.AdmissionId,
            AdmissionDate = admission.AdmissionDate,
            Status = admission.Status,
            StudentId = admission.StudentId,
            StudentName = admission.Student.StudentName,
            UserId = admission.Student.UserId,
            CourseId = admission.CourseId,
            CourseName = admission.Course.CourseName,
            TotalCourseFee = admission.Course.FeesAmount,
            TotalPaid = totalPaid,
            BalanceAmount = Math.Max(admission.Course.FeesAmount - totalPaid, 0)
        };
    }
}
