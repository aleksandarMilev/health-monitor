using HealthMonitor.Api.Extensions;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Personal, git-ignored overrides (e.g. a real API key).
// Optional: the app runs without it.
builder
    .Configuration
    .AddJsonFile(
        "appsettings.Local.json",
        optional: true);

builder
    .Services
    .ConfigureOptions(builder.Configuration)
    .AddInMemoryDb()
    .AddServices()
    .AddExternalApiHttpClient()
    .AddOpenApi()
    .AddProblemDetails()
    .AddControllers();

var app = builder.Build();

var cancellationToken = app
    .Lifetime
    .ApplicationStopping;

await app.SeedData(cancellationToken);

app.UseExceptionHandler();
app.UseStatusCodePages();

app.MapOpenApi();
app.MapScalarApiReference();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapControllers();

await app.RunAsync(cancellationToken);
