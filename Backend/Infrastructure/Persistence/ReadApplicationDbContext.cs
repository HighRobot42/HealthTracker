using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Domain.Entities.DailyRecordAggregate;
using Application.Common.Interfaces;

namespace Infrastructure.Persistence;

/// <summary>
/// Read-side DbContext. NoTracking by default — optimised for queries and projections.
/// Used by HotChocolate GraphQL via AddProjections() + AddFiltering() + AddSorting().
/// </summary>
public sealed class ReadApplicationDbContext(
    DbContextOptions<ReadApplicationDbContext> options)
    : DbContext(options), IReadApplicationDbContext
{
    public DbSet<DailyRecord> DailyRecords { get; set; } = null!;

    public ReadApplicationDbContext() : this(new DbContextOptions<ReadApplicationDbContext>()) { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Ensure NoTracking globally for this context
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(builder);
    }
}
