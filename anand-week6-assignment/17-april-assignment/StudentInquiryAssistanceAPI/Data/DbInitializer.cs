using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StudentInquiryAssistanceAPI.Constants;
using StudentInquiryAssistanceAPI.Models;

namespace StudentInquiryAssistanceAPI.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
        var passwordHasher = serviceProvider.GetRequiredService<IPasswordHasher<User>>();

        await context.Database.EnsureCreatedAsync();

        if (!await context.Users.AnyAsync(u => u.Email == "admin@niit.com"))
        {
            var admin = new User
            {
                Email = "admin@niit.com",
                Username = "admin",
                MobileNumber = "9999999999",
                UserRole = AppRoles.Admin
            };
            admin.PasswordHash = passwordHasher.HashPassword(admin, "Admin@123");
            context.Users.Add(admin);
        }

        if (!await context.Users.AnyAsync(u => u.Email == "staff@niit.com"))
        {
            var officeStaff = new User
            {
                Email = "staff@niit.com",
                Username = "officestaff",
                MobileNumber = "8888888888",
                UserRole = AppRoles.OfficeStaff
            };
            officeStaff.PasswordHash = passwordHasher.HashPassword(officeStaff, "Staff@123");
            context.Users.Add(officeStaff);
        }

        await context.SaveChangesAsync();

        var studentUser = await context.Users
            .Include(u => u.Student)
            .FirstOrDefaultAsync(u => u.Email == "student1@niit.com");

        if (studentUser is null)
        {
            studentUser = new User
            {
                Email = "student1@niit.com",
                Username = "student1",
                MobileNumber = "9876543210",
                UserRole = AppRoles.Student
            };
            studentUser.PasswordHash = passwordHasher.HashPassword(studentUser, "Stud123");
            context.Users.Add(studentUser);
            await context.SaveChangesAsync();
        }

        if (studentUser.Student is null)
        {
            context.Students.Add(new Student
            {
                StudentName = "Student One",
                StudentEmailId = "student1@niit.com",
                UserId = studentUser.UserId
            });
        }

        if (!await context.Courses.AnyAsync())
        {
            var adminUserId = await context.Users
                .Where(u => u.Email == "admin@niit.com")
                .Select(u => u.UserId)
                .FirstAsync();

            context.Courses.AddRange(
                new Course
                {
                    CourseName = "Java Full Stack",
                    Description = "Full stack development with Java, Spring Boot, Angular, and SQL.",
                    InstructorName = "NIIT Faculty",
                    Duration = "6 Months",
                    FeesAmount = 30000,
                    CreatedByUserId = adminUserId
                },
                new Course
                {
                    CourseName = ".NET Full Stack",
                    Description = "End-to-end development using ASP.NET Core, Web API, Angular, and SQL Server.",
                    InstructorName = "NIIT Faculty",
                    Duration = "6 Months",
                    FeesAmount = 32000,
                    CreatedByUserId = adminUserId
                },
                new Course
                {
                    CourseName = "Data Analytics",
                    Description = "Training on Excel, SQL, Python, Power BI, and analytics fundamentals.",
                    InstructorName = "Analytics Trainer",
                    Duration = "4 Months",
                    FeesAmount = 25000,
                    CreatedByUserId = adminUserId
                });
        }

        await context.SaveChangesAsync();
    }
}
