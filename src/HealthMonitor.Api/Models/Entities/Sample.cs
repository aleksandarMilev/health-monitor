namespace HealthMonitor.Api.Models.Entities;

public class Sample
{
    public int Id { get; init; }

    public string Name { get; set; } = "";

    public double Value { get; set; }

    public string? Unit { get; set; }

    public DateTime RecordedAt { get; set; }
}
