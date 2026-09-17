using BeCause.NuGet.HealthCheck.Extensions.Endpoints;
using BeCause.NuGet.HealthCheck.Extensions.ServiceCollection;
using Infrastructure.Persistence;

namespace HealthTracker.Api.GraphQl;

internal partial class Startup
{
    private static void RegisterHealthChecks(IServiceCollection services, IConfiguration configuration) =>

        services.AddHealthChecks()
            .AddRabbitMqHealthCheck(configuration.GetConnectionString("RabbitMq") ?? throw new ArgumentNullException("RabbitMq connection string is not configured."))
            .AddWriteDbContextHealthCheck<WriteApplicationDbContext>()
            .AddReadDbContextHealthCheck<ReadApplicationDbContext>()
            .AddPgSqlWriteDatabaseHealthCheck(configuration.GetConnectionString("WikiArbitrationWriteConnection") ??
                                              configuration.GetConnectionString("DefaultConnection") ?? throw new ArgumentNullException("WriteConnection connection string is not configured."))
            .AddPgSqlReadDatabaseHealthCheck(configuration.GetConnectionString("WikiArbitrationReadConnection") ??
                                             configuration.GetConnectionString("DefaultConnection") ?? throw new ArgumentNullException("ReadConnection connection string is not configured."))
            .AddKubernetesLivenessCheck();

    private void ConfigureHealthCheckPipeline(IApplicationBuilder app) =>
        app.UseGeneralHealthCheck();
}
