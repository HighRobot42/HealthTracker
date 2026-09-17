using Application.Common.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using System.Diagnostics;

namespace Infrastructure;

public static partial class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton(TimeProvider.System);

        AddDbContexts(services, configuration);
        AddCache(services, configuration);

        return services;
    }
    private static void AddCache(IServiceCollection services, IConfiguration configuration)
    {
        // -- Redis --------------------------------------------------------------
        var redisCs = configuration.GetConnectionString("Redis")
            ?? throw new InvalidOperationException("Redis connection string is not configured.");
        services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(redisCs));
        // services.AddScoped(typeof(ICacheRepository<>), typeof(ProductCacheRepository)); 
    }
    private static void AddDbContexts(IServiceCollection services, IConfiguration configuration)
    {
        string? defaultConnectionString = configuration.GetConnectionString("DefaultConnection");

        string? readConnectionString =
            configuration.GetConnectionString("HealthTrackerReadConnection")
            ?? defaultConnectionString ?? throw new ArgumentNullException("HealthTrackerReadConnection");
        string? writeConnectionString =
            configuration.GetConnectionString("HealthTrackerWriteConnection")
            ?? defaultConnectionString ?? throw new ArgumentNullException("HealthTrackerWriteConnection");

        services.AddDbContext<ReadApplicationDbContext>(options =>
        {
            options.LogTo(message => Debug.WriteLine(message));
            options.UseNpgsql(
                readConnectionString,
                b =>
                {
                    b.EnableRetryOnFailure(
                        maxRetryCount: 3,
                        maxRetryDelay: TimeSpan.FromSeconds(5),
                        errorCodesToAdd: null);
                    b.CommandTimeout(30); // 30 second command timeout
                });
        });

        services.AddDbContext<WriteApplicationDbContext>
        (options =>
        {
            options.LogTo(message => Debug.WriteLine(message));
            options.UseNpgsql(
                writeConnectionString,
                b =>
                {
                    b.EnableRetryOnFailure(
                        maxRetryCount: 3,
                        maxRetryDelay: TimeSpan.FromSeconds(5),
                        errorCodesToAdd: null);
                    b.CommandTimeout(30); // 30 second command timeout
                });
        });

        services.AddScoped<IReadApplicationDbContext>(provider =>
            provider.GetRequiredService<ReadApplicationDbContext>());

        services.AddScoped<IWriteApplicationDbContext>(provider =>
            provider.GetRequiredService<WriteApplicationDbContext>());
    }
}
