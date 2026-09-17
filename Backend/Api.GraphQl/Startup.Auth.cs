using BeCause.NuGet.IdentityServer.Authentication;
using BeCause.NuGet.IdentityServer.Configuration.ServiceToService;

namespace HealthTracker.Api.GraphQl;

internal partial class Startup
{
    private void ConfigureAuthentication(IServiceCollection services)
    {
        services.AddGraphQLAuthentication(
            Configuration.GetSection("IdentityServer"),
            options =>
            {
                options.UsePermissionBasedAuthorization = true;
            }
        );

        services.Configure<Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerOptions>(
            Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme,
            options => options.RequireHttpsMetadata = false
        );
        services.AddBeCauseServiceClient(
            Configuration.GetSection(ServiceClientOptions.SectionName)
        );
        services.AddHttpContextAccessor();
    }

    private void ConfigureAuthenticationPipeline(IApplicationBuilder app)
    {
        app.UseBeCauseAuthentication(options =>
        {
            options.EnableImpersonation = false;
        });
    }
}
