namespace HealthMonitor.Api.Models.Dtos;

public class SampleResponseDto
{
    public int Id { get; init; }

    public string Name { get; init; } = "";

    public double Value { get; init; }

    public string? Unit { get; init; }

    public DateTime RecordedAt { get; init; }
}
