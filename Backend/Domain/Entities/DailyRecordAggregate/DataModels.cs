namespace Domain.Entities.DailyRecordAggregate;

public class FoodData
{
    public List<string> Ingredients { get; set; } = new();
    public decimal TotalCalories { get; set; }
    public decimal ProteinGrams { get; set; }
    public decimal CarbsGrams { get; set; }
    public decimal FatGrams { get; set; }
}

public class LocationEnvironmentData
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string? WeatherCondition { get; set; }
    public double TemperatureCelsius { get; set; }
    public double AirQualityIndexPm25 { get; set; }
}

public class MoodFeelingData
{
    public int MoodScore { get; set; } // 1 to 10
    public List<string> Tags { get; set; } = new();
    public string? Notes { get; set; }
}

public class MedicationData
{
    public List<string> TakenDrugs { get; set; } = new();
    public List<string> ActiveChemicals { get; set; } = new();
}
