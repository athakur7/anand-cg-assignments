using Microsoft.EntityFrameworkCore;
using ModifiedProject.Models;
using ModifiedProject.Services;
namespace ModifiedProject
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ✅ Add DB Connection
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection")));

            // ✅ Required Services
            builder.Services.AddHttpContextAccessor();

            // Add controllers
            builder.Services.AddControllers();

            // Swagger
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddScoped<IEmployee, EmployeeService>();
            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            // (later we will add authentication here)
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}