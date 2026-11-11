namespace HealthMonitor.Api.Data.Seeding;

using Api.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Models.Entities;
using Models.Seed;

public class DataSeeder(
    AppDbContext data,
    JsonLoader jsonLoader,
    CsvLoader csvLoader,
    IOptions<SeedingOptions> options,
    ILogger<DataSeeder> logger)
{
    private readonly SeedingOptions options = options.Value;

    public async Task Seed(
        CancellationToken cancellationToken = default)
    {
        await SeedSamplesAsync(cancellationToken);
        //add more seeders here.
    }

    private async Task SeedSamplesAsync(
        CancellationToken cancellationToken = default)
    {
        if (await data.Samples.AnyAsync(cancellationToken))
            return;

        var rows = new List<SampleSeedRow>();

        if (this.options.UseJson)
        {
            var json = await jsonLoader.Load<SampleSeedRow>(
                this.options.JsonFile,
                cancellationToken);

            rows.AddRange(json);
        }

        if (this.options.UseCsv)
        {
            var csv = await csvLoader.Load<SampleSeedRow>(
                this.options.CsvFile,
                cancellationToken);

            rows.AddRange(csv);
        }

        var entities = rows
            .Select(r => new Sample
            {
                Name = r.Name,
                Value = r.Value,
                Unit = r.Unit,
                RecordedAt = r.RecordedAt,
            });

        data.AddRange(entities);
        var saved = await data.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Seeded {Count} samples (json: {Json}, csv: {Csv})",
            saved,
            this.options.UseJson,
            this.options.UseCsv);
    }
}
