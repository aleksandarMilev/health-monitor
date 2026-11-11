namespace HealthMonitor.Api.Services.External;

public interface IExternalApiClient
{
    bool IsConfigured { get; }

    string? ConfigurationError { get; }

    Task<T?> Get<T>(
        string relativePath,
        CancellationToken cancellationToken = default);

    Task<string> GetRaw(
        string relativePath,
        CancellationToken cancellationToken = default);
}