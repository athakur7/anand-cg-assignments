using Microsoft.Azure.Cosmos;
using MVC_Demo_Project.Data;

namespace cosmodbdemo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddSingleton<CosmosDbService>(options =>
            {
                var configuration = builder.Configuration;

                var endpoint = configuration["CosmosDb:Endpoint"];
                var primaryKey = configuration["CosmosDb:PrimaryKey"];
                var configuredDatabase = configuration["CosmosDb:DatabaseName"] ?? "DemoDB";
                var configuredContainer = configuration["CosmosDb:ContainerName"] ?? "items";

                var cosmosClient = new CosmosClient(endpoint, primaryKey);

                static bool DatabaseExists(CosmosClient client, string databaseName)
                {
                    try
                    {
                        client.GetDatabase(databaseName).ReadAsync().GetAwaiter().GetResult();
                        return true;
                    }
                    catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        return false;
                    }
                }

                static bool ContainerExists(CosmosClient client, string databaseName, string containerName)
                {
                    try
                    {
                        client.GetContainer(databaseName, containerName).ReadContainerAsync().GetAwaiter().GetResult();
                        return true;
                    }
                    catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        return false;
                    }
                }

                var databaseCandidates = new[] { configuredDatabase, "CustomerOrders" }.Distinct(StringComparer.OrdinalIgnoreCase);
                var containerCandidates = new[]
                {
                    configuredContainer,
                    configuredContainer.ToLowerInvariant(),
                    configuredContainer.ToUpperInvariant(),
                    "Items",
                    "items",
                    "People",
                    "Orders"
                }.Distinct(StringComparer.OrdinalIgnoreCase);

                string? resolvedDatabase = null;
                string? resolvedContainer = null;

                foreach (var db in databaseCandidates)
                {
                    if (!DatabaseExists(cosmosClient, db))
                    {
                        continue;
                    }

                    foreach (var container in containerCandidates)
                    {
                        if (ContainerExists(cosmosClient, db, container))
                        {
                            resolvedDatabase = db;
                            resolvedContainer = container;
                            break;
                        }
                    }

                    if (resolvedDatabase is not null)
                    {
                        break;
                    }
                }

                if (resolvedDatabase is null || resolvedContainer is null)
                {
                    throw new InvalidOperationException("No usable Cosmos DB database/container found. Create a container first or update CosmosDb settings.");
                }

                return new CosmosDbService(cosmosClient, resolvedDatabase, resolvedContainer);
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
