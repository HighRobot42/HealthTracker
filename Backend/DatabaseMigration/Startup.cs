using Application;
using BeCause.NuGet.Core.Application.Configurations;
using BeCause.NuGet.DatabaseMigration.Extensions;
using BeCause.NuGet.Telemetry.Extensions;
using Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HealthTracker.DatabaseMigration;

public class Startup
{
    public Startup(IConfiguration configuration, IHostEnvironment environment)
    {
        Configuration = configuration;
        Environment = environment;
        Configuration["DatabaseMigration:ConnectionString"] = Configuration["ConnectionStrings:DefaultConnection"];
    }
    private IConfiguration Configuration { get; }
    public IHostEnvironment Environment { get; }

    public virtual void ConfigureServices(IServiceCollection services)
    {
        services.AddServiceOptions(Configuration);
        services.ConfigureOpenTelemetry(Configuration, Environment);

        services.ConfigureDbMigrationServices(Configuration);
        services.AddApplicationServices(Configuration);

        services.AddInfrastructure(Configuration);
    }

    public void Configure(IApplicationBuilder app)
    {
    }
}
