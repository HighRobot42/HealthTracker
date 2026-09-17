using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Infrastructure.Persistence;

namespace IntegrationTests.Core;

/// <summary>
/// Shared WebApplicationFactory fixture.
/// Replaces EF Core DbContexts with InMemory for integration tests — no real DB required.
/// Extend this class to swap out additional services (e.g., Redis mock, MassTransit InMemory).
/// </summary>
public class WebApplicationFixture<TStartup> : WebApplicationFactory<TStartup>
    where TStartup : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove real EF Core registrations
            RemoveDbContext<WriteApplicationDbContext>(services);
            RemoveDbContext<ReadApplicationDbContext>(services);

            // Add InMemory replacements
            var dbName = $"TestDb_{Guid.NewGuid()}";
            services.AddDbContext<WriteApplicationDbContext>(opts =>
                opts.UseInMemoryDatabase(dbName));
            services.AddDbContext<ReadApplicationDbContext>(opts =>
                opts.UseInMemoryDatabase(dbName));
        });
    }

    private static void RemoveDbContext<TContext>(IServiceCollection services)
        where TContext : DbContext
    {
        var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<TContext>));
        if (descriptor is not null) services.Remove(descriptor);
    }
}
