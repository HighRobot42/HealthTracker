using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Domain.Entities.DailyRecordAggregate;

namespace Application.Common.Interfaces;

/// <summary>
/// Write-side DbContext interface (CQRS). Exposed only to command handlers.
/// Implementation lives in Infrastructure and is injected via DI.
/// </summary>
public interface IWriteApplicationDbContext
{
    DbSet<DailyRecord> DailyRecords { get; set; }

    DbSet<TEntity> Set<TEntity>() where TEntity : class;
    Task<int>      SaveChangesAsync(CancellationToken cancellationToken);
    EntityEntry    Entry(object entity);
    EntityEntry<T> Attach<T>(T entity) where T : class;
}
