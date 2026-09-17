using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System.Reflection;

namespace HealthTracker.DatabaseMigration.Persistence;

// ReSharper disable once UnusedMember.Global
public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<WriteApplicationDbContext>
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

        return new WriteApplicationDbContext(optionsBuilder.Options, null, null, null);
    }
}
