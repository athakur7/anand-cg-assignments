using Microsoft.EntityFrameworkCore;
using ProductAPI.Data;
using ProductAPI.Services;

namespace ProductAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Configuration.AddUserSecrets<Program>(optional: true);

            // Add services to the container.
            var azureConnectionString = builder.Configuration["ConnectionStrings:AzureSqlConnection"];
            var defaultConnectionString = builder.Configuration["ConnectionStrings:DefaultConnection"];
            var connectionString = !string.IsNullOrWhiteSpace(azureConnectionString)
                ? azureConnectionString
                : defaultConnectionString;

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("Connection string not found. Set 'ConnectionStrings:AzureSqlConnection' (or 'ConnectionStrings:DefaultConnection') in user-secrets.");
            }

            builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
