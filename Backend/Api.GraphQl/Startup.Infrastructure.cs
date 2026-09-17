using Application;
using BeCause.NuGet.Core.Application.Configurations;
using BeCause.NuGet.Core.Domain.Converters;
using BeCause.NuGet.Exceptions.Api;
using BeCause.NuGet.IdentityServer.Authentication;
using BeCause.NuGet.IdentityServer.Configuration.ServiceToService;
using BeCause.NuGet.Versioning;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace HealthTracker.Api.GraphQl;

internal partial class Startup
{
    private void ConfigureInfrastructure(IServiceCollection services)
    {
        services.AddServiceOptions(Configuration);

        services.AddApplicationServices(Configuration);
        services.AddInfrastructure(Configuration);
    }
}
