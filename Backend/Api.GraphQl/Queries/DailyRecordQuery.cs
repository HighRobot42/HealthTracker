using Application.Common.Interfaces;
using Domain.Entities.DailyRecordAggregate;
using HotChocolate.Data;

namespace HealthTracker.Api.GraphQl.Queries;

public class DailyRecordQuery
{
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<DailyRecord> GetDailyRecords([Service] IReadApplicationDbContext context)
    {
        return context.DailyRecords;
    }
}
