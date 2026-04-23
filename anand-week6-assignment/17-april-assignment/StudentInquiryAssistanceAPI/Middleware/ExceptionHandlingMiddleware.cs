using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using StudentInquiryAssistanceAPI.Data;
using StudentInquiryAssistanceAPI.Exceptions;
using StudentInquiryAssistanceAPI.Models;

namespace StudentInquiryAssistanceAPI.Middleware;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context, ApplicationDbContext dbContext)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Unhandled exception while processing {Path}", context.Request.Path);

            var statusCode = exception is AppException appException
                ? appException.StatusCode
                : StatusCodes.Status500InternalServerError;

            try
            {
                dbContext.ErrorLogs.Add(new ErrorLog
                {
                    Message = exception.Message,
                    StackTrace = exception.ToString(),
                    Path = context.Request.Path,
                    StatusCode = statusCode,
                    LoggedAt = DateTime.Now
                });
                await dbContext.SaveChangesAsync();
            }
            catch (Exception logException)
            {
                logger.LogError(logException, "Failed to persist error log.");
            }

            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            var response = new
            {
                message = exception.Message,
                statusCode
            };

            var payload = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(payload);
        }
    }
}
