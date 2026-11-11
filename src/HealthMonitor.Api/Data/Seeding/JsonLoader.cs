namespace HealthMonitor.Api.Data.Seeding;

using System.Text.Json;

public class JsonLoader(ILogger<JsonLoader> logger)
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true,
    };

    public async Task<List<T>> Load<T>(
        string fileName,
        CancellationToken cancellationToken = default)
    {
        var path = SeedFilePaths.Resolve(fileName);
        if (!File.Exists(path))
        {
            logger.LogWarning("JSON seed file not found: {Path}", path);
            return [];
        }

        await using var stream = File.OpenRead(path);
        var rows = await JsonSerializer.DeserializeAsync<List<T>>(
            stream,
            Options,
            cancellationToken);

        logger.LogInformation(
            "Loaded {Count} rows from {File}",
            rows?.Count ?? 0,
            fileName);

        return rows ?? [];
    }
}
