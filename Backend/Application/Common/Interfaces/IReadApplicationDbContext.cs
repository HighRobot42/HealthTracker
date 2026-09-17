using Microsoft.EntityFrameworkCore;
using Domain.Entities.DailyRecordAggregate;

namespace Application.Common.Interfaces;

/// <summary>
/// Read-side DbContext interface (CQRS). Used only by query handlers.
/// The implementation is registered with QueryTrackingBehavior.NoTracking.
/// </summary>
public interface IReadApplicationDbContext
{
    DbSet<DailyRecord> DailyRecords { get; set; }

    DbSet<TEntity> Set<TEntity>() where TEntity : class;
}
