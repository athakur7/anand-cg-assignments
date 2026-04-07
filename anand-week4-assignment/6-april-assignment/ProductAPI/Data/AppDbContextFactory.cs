using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ProductAPI.Data
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        private const string UserSecretsId = "f698dc7f-f36a-407f-8aad-4222f2e89494";

        public AppDbContext CreateDbContext(string[] args)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
            var secretsPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "Microsoft",
                "UserSecrets",
                UserSecretsId,
                "secrets.json");

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile($"appsettings.{environment}.json", optional: true)
                .AddJsonFile(secretsPath, optional: true)
                .AddUserSecrets<Program>(optional: true)
                .AddEnvironmentVariables()
                .Build();

            var azureConnectionString = configuration["ConnectionStrings:AzureSqlConnection"];
            var defaultConnectionString = configuration["ConnectionStrings:DefaultConnection"];
            var connectionString = !string.IsNullOrWhiteSpace(azureConnectionString)
                ? azureConnectionString
                : defaultConnectionString;

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("Connection string not found for design-time. Set 'ConnectionStrings:AzureSqlConnection' (or 'ConnectionStrings:DefaultConnection') in user-secrets.");
            }

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
