using Application.Common.Interfaces;
using BeCause.NuGet.IdentityBase.Services;
using Domain.Entities.DailyRecordAggregate;

namespace HealthTracker.Api.GraphQl.Mutations;

public class Mutation
{
    public async Task<DailyEntryPayload> SubmitDailyEntryAsync(
        DailyEntryInput input,
        [Service] IWriteApplicationDbContext dbContext,
        [Service] ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId;
        if (string.IsNullOrEmpty(userId))
        {
            userId = Guid.NewGuid().ToString(); // Mocking user id for now
        }

        var record = DailyRecord.Create(
            Guid.NewGuid(),
            userId,
            DateTime.UtcNow,
            input.RawInput);

        dbContext.DailyRecords.Add(record);
        await dbContext.SaveChangesAsync(cancellationToken);

        // Later: publish event to MassTransit to trigger background job

        return new DailyEntryPayload(record.Id, true);
    }
}

public record DailyEntryInput(string RawInput);
public record DailyEntryPayload(Guid RecordId, bool Success);
