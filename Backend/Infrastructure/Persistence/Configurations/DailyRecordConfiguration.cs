using Domain.Entities.DailyRecordAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class DailyRecordConfiguration : IEntityTypeConfiguration<DailyRecord>
{
    public void Configure(EntityTypeBuilder<DailyRecord> builder)
    {
        builder.ToTable("DailyRecords");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.UserId).IsRequired();
        builder.Property(x => x.RecordDate).IsRequired();
        builder.Property(x => x.RawInput).IsRequired();

        builder.OwnsOne(x => x.FoodData, b => { b.ToJson(); });
        builder.OwnsOne(x => x.LocationEnvironmentData, b => { b.ToJson(); });
        builder.OwnsOne(x => x.MoodFeelingData, b => { b.ToJson(); });
        builder.OwnsOne(x => x.MedicationData, b => { b.ToJson(); });
    }
}
