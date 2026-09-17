using Asp.Versioning.ApiExplorer;
using static HotChocolate.Types.DirectiveNames;

namespace HealthTracker.Api.GraphQl;

internal partial class Startup
{
    internal Startup(IConfiguration configuration, IHostEnvironment environment)
    {
        Configuration = configuration;
        Environment = environment;
    }

    internal IConfiguration Configuration { get; }
    internal IHostEnvironment Environment { get; }

    internal void ConfigureServices(IServiceCollection services)
    {
        ConfigureInfrastructure(services);
        ConfigureWebServices(services);
        ConfigureAuthentication(services);
        ConfigureTelemetryPipeline(services);
        RegisterHealthChecks(services, Configuration);
    }

    internal void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
    { 
        ConfigureWebPipeline(app, env, provider);
        ConfigureAuthenticationPipeline(app);
        ConfigureHealthCheckPipeline(app);
    }
}


