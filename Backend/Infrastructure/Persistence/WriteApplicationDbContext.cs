using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using BeCause.NuGet.IdentityBase.Services;
using Domain.Entities.SeedWork;
using Domain.Entities.DailyRecordAggregate;
using Application.Common.Interfaces;

namespace Infrastructure.Persistence;

/// <summary>
/// Write-side DbContext. Wraps every save in an execution strategy + transaction,
/// then publishes domain events via MediatR IPublisher after commit.
/// </summary>
public sealed class WriteApplicationDbContext(
    DbContextOptions<WriteApplicationDbContext> options,
    ICurrentUserService currentUserService,
    IPublisher publisher,
    TimeProvider timeProvider)
    : DbContext(options), IWriteApplicationDbContext
{
    public DbSet<DailyRecord> DailyRecords { get; set; } = null!;

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // -- Audit tracking ------------------------------------------------------
        var now    = timeProvider.GetUtcNow().UtcDateTime;
        var userId = currentUserService.UserId;

        foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedBy  = userId;
                    entry.Entity.CreatedAt  = now;
                    entry.Entity.LastModifiedBy = userId;
                    entry.Entity.LastModifiedAt = now;
                    break;
                case EntityState.Modified:
                    entry.Entity.LastModifiedBy = userId;
                    entry.Entity.LastModifiedAt = now;
                    break;
            }
        }

        // -- Collect domain events before save -----------------------------------
        var aggregates = ChangeTracker.Entries<AggregateRoot>()
            .Select(e => e.Entity)
            .Where(e => e.DomainEvents.Count != 0)
            .ToList();

        // -- Execution strategy + transaction -------------------------------------
        var strategy = Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var result = await base.SaveChangesAsync(cancellationToken);

                // -- Publish domain events after commit --------------------------
                foreach (var aggregate in aggregates)
                {
                    var events = aggregate.PopDomainEvents();
                    foreach (var domainEvent in events)
                        await publisher.Publish(domainEvent, cancellationToken);
                }

                await transaction.CommitAsync(cancellationToken);
                return result;
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        });
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(builder);
    }
}
