namespace HealthMonitor.Api.Extensions;

using Data.Seeding;

public static class WebApplicationExtensions
{
    extension(WebApplication app)
    {
        public async Task SeedData(
            CancellationToken cancellationToken = default)
        {
            using var scope = app.Services.CreateScope();

            var seeder = scope
                .ServiceProvider
                .GetRequiredService<DataSeeder>();

            await seeder.Seed(cancellationToken);
        }
    }
}
