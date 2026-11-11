namespace HealthMonitor.Api.Extensions;

using Data;
using Data.Seeding;
using Microsoft.EntityFrameworkCore;
using Options;
using Services.External;
using Services.Samples;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection ConfigureOptions(
            IConfiguration config)
            => services
                .Configure<ExternalApiOptions>(
                    config.GetSection(ExternalApiOptions.SectionName))
                .Configure<SeedingOptions>(
                    config.GetSection(SeedingOptions.SectionName));

        public IServiceCollection AddInMemoryDb()
        {
            const string DbName = "AppDb";

            return services
                .AddDbContext<AppDbContext>(
                    options => options.UseInMemoryDatabase(DbName));
        }

        public IServiceCollection AddServices()
            => services
                .AddScoped<JsonLoader>()
                .AddScoped<CsvLoader>()
                .AddScoped<DataSeeder>()
                .AddScoped<ISampleService, SampleService>();

        public IServiceCollection AddExternalApiHttpClient()
        {
            services
                .AddHttpClient<IExternalApiClient, ExternalApiClient>();

            return services;
        }
    }
}
