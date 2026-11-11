namespace HealthMonitor.Api.Options;

public class SeedingOptions
{
    public const string SectionName = "Seeding";

    public bool UseJson { get; set; } = true;

    public string JsonFile { get; set; } = "samples.json";

    public bool UseCsv { get; set; } = false;

    public string CsvFile { get; set; } = "samples.csv";
}
