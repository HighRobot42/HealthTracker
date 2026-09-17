using BeCause.NuGet.Telemetry.Extensions;

namespace HealthTracker.Api.GraphQl;

internal partial class Startup
{
    private void ConfigureTelemetryPipeline(IServiceCollection services)
    {
        services.ConfigureOpenTelemetry(Configuration, Environment);
    }
}
