using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Application.Common.Behaviours;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services, IConfiguration configuration)
    {
        // -- MediatR -------------------------------------------------------------
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));

        // -- Pipeline behaviours (order matters) ---------------------------------
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehaviour<,>));

        // -- FluentValidation (auto-discover from assembly) -----------------------
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        return services;
    }
}
