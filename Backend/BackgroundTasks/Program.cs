using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;
using HealthTracker.Infrastructure.ServiceRegistrations;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((ctx, services) =>
    {
        services.AddInfrastructure(
            ctx.Configuration,
            configureConsumersEndpoints: null,
            consumerAssemblies: Assembly.GetExecutingAssembly());
    })
    .Build();

await host.RunAsync();
