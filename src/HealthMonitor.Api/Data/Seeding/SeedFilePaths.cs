namespace HealthMonitor.Api.Data.Seeding;

public static class SeedFilePaths
{
    public static string Resolve(string fileName) =>
        Path.Combine(
            AppContext.BaseDirectory,
            "Data",
            "Files",
            fileName);
}
