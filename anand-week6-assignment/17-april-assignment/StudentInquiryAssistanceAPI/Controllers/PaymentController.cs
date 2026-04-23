using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentInquiryAssistanceAPI.Constants;
using StudentInquiryAssistanceAPI.Data;
using StudentInquiryAssistanceAPI.DTOs.Payments;
using StudentInquiryAssistanceAPI.Extensions;
using StudentInquiryAssistanceAPI.Models;

namespace StudentInquiryAssistanceAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentController(ApplicationDbContext context) : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.OfficeStaff}")]
    public async Task<IActionResult> GetAllPaymentHistory()
    {
        var payments = await context.Payments
            .AsNoTracking()
            .Include(p => p.Student)
            .ThenInclude(s => s.User)
            .Include(p => p.Course)
            .OrderByDescending(p => p.PaymentDate)
            .ToListAsync();

        return Ok(payments.Select(MapPayment));
    }

    [HttpGet("{paymentId:int}")]
    [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.OfficeStaff},{AppRoles.Student}")]
    public async Task<IActionResult> GetById(int paymentId)
    {
        var payment = await context.Payments
            .AsNoTracking()
            .Include(p => p.Student)
            .ThenInclude(s => s.User)
            .Include(p => p.Course)
            .FirstOrDefaultAsync(p => p.PaymentId == paymentId);

        if (payment is null)
        {
            return NotFound(new { message = "Payment not found." });
        }

        if (User.IsInRole(AppRoles.Student) && payment.Student.UserId != User.GetRequiredUserId())
        {
            return Forbid();
        }

        return Ok(MapPayment(payment));
    }

    [HttpGet("history/user/{userId:long}")]
    [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.OfficeStaff},{AppRoles.Student}")]
    public async Task<IActionResult> GetPaymentHistoryByUser(long userId)
    {
        if (User.IsInRole(AppRoles.Student) && userId != User.GetRequiredUserId())
        {
            return Forbid();
        }

        var payments = await context.Payments
            .AsNoTracking()
            .Include(p => p.Student)
            .ThenInclude(s => s.User)
            .Include(p => p.Course)
            .Where(p => p.Student.UserId == userId)
            .OrderByDescending(p => p.PaymentDate)
            .ToListAsync();

        return Ok(payments.Select(MapPayment));
    }

    [HttpGet("history/me")]
    [Authorize(Roles = AppRoles.Student)]
    public async Task<IActionResult> GetMyPaymentHistory()
    {
        var userId = User.GetRequiredUserId();
        return await GetPaymentHistoryByUser(userId);
    }

    [HttpGet("balance/{admissionId:int}")]
    [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.OfficeStaff},{AppRoles.Student}")]
    public async Task<IActionResult> GetBalanceAmount(int admissionId)
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

        return Ok(MapBalance(admission));
    }

    [HttpPost]
    [Authorize(Roles = AppRoles.Student)]
    public async Task<IActionResult> MakePayment(CreatePaymentDto request)
    {
        var studentId = User.GetRequiredStudentId();

        var admission = await context.Admissions
            .Include(a => a.Student)
            .ThenInclude(s => s.User)
            .Include(a => a.Course)
            .Include(a => a.Payments)
            .FirstOrDefaultAsync(a => a.AdmissionId == request.AdmissionId);

        if (admission is null)
        {
            return NotFound(new { message = "Admission not found." });
        }

        if (admission.StudentId != studentId)
        {
            return Forbid();
        }

        if (admission.Status == AdmissionStatuses.Rejected || admission.Status == AdmissionStatuses.Closed)
        {
            return BadRequest(new { message = "Payment cannot be made for this admission in its current status." });
        }

        var totalPaid = admission.Payments.Sum(p => p.Amount);
        var balance = admission.Course.FeesAmount - totalPaid;

        if (balance <= 0)
        {
            return BadRequest(new { message = "No balance amount is pending for this admission." });
        }

        if (request.Amount > balance)
        {
            return BadRequest(new { message = $"Payment amount cannot exceed the balance amount of {balance}." });
        }

        var payment = new Payment
        {
            AdmissionId = admission.AdmissionId,
            StudentId = studentId,
            CourseId = admission.CourseId,
            Amount = request.Amount,
            PaymentMode = request.PaymentMode.Trim(),
            PaymentDate = DateTime.Now
        };

        context.Payments.Add(payment);
        await context.SaveChangesAsync();

        payment = await context.Payments
            .AsNoTracking()
            .Include(p => p.Student)
            .ThenInclude(s => s.User)
            .Include(p => p.Course)
            .FirstAsync(p => p.PaymentId == payment.PaymentId);

        return CreatedAtAction(nameof(GetById), new { paymentId = payment.PaymentId }, MapPayment(payment));
    }

    [HttpPost("reminder/{admissionId:int}")]
    [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.OfficeStaff}")]
    public async Task<IActionResult> SendReminderMessage(int admissionId)
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

        var balance = MapBalance(admission);
        var message = balance.BalanceAmount > 0
            ? $"Reminder sent to {admission.Student.StudentName}: Pending balance amount for {admission.Course.CourseName} is {balance.BalanceAmount}."
            : $"No reminder required. The balance amount for {admission.Course.CourseName} is already cleared.";

        return Ok(new { message, balance = balance.BalanceAmount });
    }

    private static PaymentDto MapPayment(Payment payment)
    {
        return new PaymentDto
        {
            PaymentId = payment.PaymentId,
            PaymentDate = payment.PaymentDate,
            Amount = payment.Amount,
            PaymentMode = payment.PaymentMode,
            StudentId = payment.StudentId,
            StudentName = payment.Student.StudentName,
            UserId = payment.Student.UserId,
            AdmissionId = payment.AdmissionId,
            CourseId = payment.CourseId,
            CourseName = payment.Course.CourseName
        };
    }

    private static BalanceDueDto MapBalance(Admission admission)
    {
        var totalPaid = admission.Payments.Sum(p => p.Amount);
        return new BalanceDueDto
        {
            AdmissionId = admission.AdmissionId,
            CourseId = admission.CourseId,
            CourseName = admission.Course.CourseName,
            CourseFee = admission.Course.FeesAmount,
            TotalPaid = totalPaid,
            BalanceAmount = Math.Max(admission.Course.FeesAmount - totalPaid, 0)
        };
    }
}
