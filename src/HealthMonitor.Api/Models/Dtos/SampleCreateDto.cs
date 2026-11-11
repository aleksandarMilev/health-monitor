namespace HealthMonitor.Api.Models.Dtos;

using System.ComponentModel.DataAnnotations;

public class SampleCreateDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; init; } = "";

    [Required]
    public double? Value { get; init; }

    [StringLength(20)]
    public string? Unit { get; init; }

    public DateTime? RecordedAt { get; init; }
}
