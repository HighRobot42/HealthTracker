using Domain.Entities.SeedWork;

namespace Domain.Entities.DailyRecordAggregate;

public sealed class DailyRecord : AggregateRoot
{
    public string UserId { get; private set; }
    public DateTime RecordDate { get; private set; }
    public string RawInput { get; private set; } = null!;
    
    // JSONB columns mapping
    public FoodData? FoodData { get; private set; }
    public LocationEnvironmentData? LocationEnvironmentData { get; private set; }
    public MoodFeelingData? MoodFeelingData { get; private set; }
    public MedicationData? MedicationData { get; private set; }

    private DailyRecord() { }

    public static DailyRecord Create(Guid id, string userId, DateTime recordDate, string rawInput)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(rawInput);

        var record = new DailyRecord
        {
            Id = id,
            UserId = userId,
            RecordDate = recordDate.Date,
            RawInput = rawInput
        };

        // Emit domain event if needed
        // record.RaiseDomainEvent(new DailyRecordCreatedEvent(id));

        return record;
    }

    public void Enrich(FoodData? food, LocationEnvironmentData? loc, MoodFeelingData? mood, MedicationData? med)
    {
        FoodData = food ?? FoodData;
        LocationEnvironmentData = loc ?? LocationEnvironmentData;
        MoodFeelingData = mood ?? MoodFeelingData;
        MedicationData = med ?? MedicationData;
    }
}
