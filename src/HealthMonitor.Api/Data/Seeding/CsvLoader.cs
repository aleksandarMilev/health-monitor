namespace HealthMonitor.Api.Data.Seeding;

using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;

public class CsvLoader(ILogger<CsvLoader> logger)
{
    private static readonly CsvConfiguration Config = new(CultureInfo.InvariantCulture)
    {
        PrepareHeaderForMatch = args => args.Header.Trim().ToLowerInvariant(),
        MissingFieldFound = null,   // optional columns may be missing
        HeaderValidated = null,     // extra columns in the file are ignored
        TrimOptions = TrimOptions.Trim,
    };

    public async Task<List<T>> Load<T>(
        string fileName,
        CancellationToken cancellationToken = default)
    {
        var path = SeedFilePaths.Resolve(fileName);
        if (!File.Exists(path))
        {
            logger.LogWarning("CSV seed file not found: {Path}", path);
            return [];
        }

        using var reader = new StreamReader(path);
        using var csv = new CsvReader(reader, Config);

        var dateOptions = csv
            .Context
            .TypeConverterOptionsCache
            .GetOptions<DateTime>();

        dateOptions.DateTimeStyle =
            DateTimeStyles.AssumeUniversal |
            DateTimeStyles.AdjustToUniversal;

        var rows = new List<T>();

        await foreach (var row in csv.GetRecordsAsync<T>(cancellationToken))
            rows.Add(row);

        logger.LogInformation(
            "Loaded {Count} rows from {File}",
            rows.Count,
            fileName);

        return rows;
    }
}
