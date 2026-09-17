using BeCause.NuGet.Core.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HealthTracker.DatabaseMigration;

public class Program
{
    public static async Task Main(string[] args)
    {
        try
        {
            using var host = Host.CreateDefaultBuilder(args)
                //.ConfigureBeCauseSecrets()
                .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>())
                .Build();


            var logger = host.Services.GetRequiredService<Microsoft.Extensions.Logging.ILogger<Program>>();
            logger.LogInformation("Host starting...");
            await host.StartAsync();

            using (var scope = host.Services.CreateScope())
            {
                var migrationService = scope.ServiceProvider.GetRequiredService<IDatabaseMigrationService>();
                var success = await migrationService.RunDatabaseMigrationAsync();
                logger.LogInformation("Database migration completed");

                var seedServices = scope.ServiceProvider.GetServices<IDatabaseSeedService>();
                foreach (var service in seedServices)
                    await service.RunDatabaseSeedAsync();
                logger.LogInformation("Database seed completed");

                // var cassandraSchemaInitializer = scope.ServiceProvider.GetRequiredService<CassandraSchemaInitializer>();
                // cassandraSchemaInitializer.EnsureSchema();
                logger.LogInformation("Cassandra schema ensured");

                logger.LogInformation(success
                    ? "Database migrated successfully"
                    : "Either no migration was required or an error occurred while migrating the database");
            }

            await host.StopAsync();
        }
        catch (Exception)
        {
            throw;
        }
    }
}
