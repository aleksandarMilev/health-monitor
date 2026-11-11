namespace HealthMonitor.Api.Models.Seed;

public class SampleSeedRow
{
    public string Name { get; init; } = "";

    public double Value { get; init; }

    public string? Unit { get; init; }

    public DateTime RecordedAt { get; init; }
}
