using BeCause.NuGet.IdentityBase.Services;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System.Reflection;
using System.Security.Claims;

public class DesignTimeWriteApplicationDbContextFactory : IDesignTimeDbContextFactory<WriteApplicationDbContext>
{
    public WriteApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<WriteApplicationDbContext>();
        optionsBuilder.UseNpgsql(
            // Connection string not required to generate the Migrations files
            b =>
            {
                b.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName);
                b.EnableRetryOnFailure();
                b.CommandTimeout((int)TimeSpan.FromMinutes(10).TotalSeconds);
            });

        // Provide dummy or mock implementations for required services
        var timeProvider = TimeProvider.System;
        var publisher = new DummyPublisher();
        var currentUserService = new DummyCurrentUserService();

        return new WriteApplicationDbContext(optionsBuilder.Options, currentUserService, publisher, timeProvider);
    }

    // Dummy implementations for design-time
    private class DummyPublisher : IPublisher
    {
        public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default) where TNotification : INotification
        => Task.CompletedTask;

        public Task Publish(object notification, CancellationToken cancellationToken = default)
        => Task.CompletedTask;
    }
    private class DummyCurrentUserService : ICurrentUserService
    {
        public string? UserId => null;

        public string UserName => string.Empty;

        public string Email => throw new NotImplementedException();

        public IList<Claim> Claims => throw new NotImplementedException();

        public IList<string> Roles => throw new NotImplementedException();

        public bool HasClaim(string type, string value)
        {
            return true;
        }

        public bool IsInRole(string role)
        {
            return true;
        }
    }
}