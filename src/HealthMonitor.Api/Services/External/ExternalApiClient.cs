namespace HealthMonitor.Api.Services.External;

using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Options;

public class ExternalApiClient : IExternalApiClient
{
    private const int MaxErrorBodyLength = 500;

    private readonly HttpClient http;
    private readonly ExternalApiOptions apiOptions;

    public ExternalApiClient(
        HttpClient http,
        IOptions<ExternalApiOptions> apiOptions)
    {
        this.http = http;
        this.apiOptions = apiOptions.Value;
        this.ConfigurationError = this.apiOptions.GetConfigurationError();

        if (!this.IsConfigured)
            return;

        var baseUrl = this.apiOptions.BaseUrl.EndsWith('/')
            ? this.apiOptions.BaseUrl
            : this.apiOptions.BaseUrl + "/";

        this.http.BaseAddress = new(baseUrl);
        this.http.Timeout = TimeSpan.FromSeconds(this.apiOptions.TimeoutSeconds);

        switch (this.apiOptions.ApiKeyLocation)
        {
            case ApiKeyLocation.Header:
                this.http
                    .DefaultRequestHeaders
                    .Add(this.apiOptions.ApiKeyName, this.apiOptions.ApiKey);
                break;

            case ApiKeyLocation.Bearer:
                this.http
                    .DefaultRequestHeaders
                    .Authorization = new("Bearer", this.apiOptions.ApiKey);
                break;
        }
    }

    public string? ConfigurationError { get; }

    public bool IsConfigured
        => this.ConfigurationError is null;

    public async Task<T?> Get<T>(
        string relativePath,
        CancellationToken cancellationToken = default)
    {
        using var response = await this.SendGet(
            relativePath,
            cancellationToken);

        return await response
            .Content
            .ReadFromJsonAsync<T>(cancellationToken);
    }

    public async Task<string> GetRaw(
        string relativePath,
        CancellationToken cancellationToken = default)
    {
        using var response = await this.SendGet(
            relativePath,
            cancellationToken);

        return await response
            .Content
            .ReadAsStringAsync(cancellationToken);
    }

    private async Task<HttpResponseMessage> SendGet(
        string relativePath,
        CancellationToken cancellationToken)
    {
        if (!this.IsConfigured)
            throw new InvalidOperationException($"External API is not configured: {this.ConfigurationError}");

        var path = this.BuildPath(relativePath);
        var response = await this.http.GetAsync(path, cancellationToken);

        if (response.IsSuccessStatusCode)
            return response;

        using (response)
        {
            var body = await response
                .Content
                .ReadAsStringAsync(cancellationToken);

            throw new HttpRequestException(
                $"External API returned {(int)response.StatusCode} {response.ReasonPhrase}: {Truncate(body)}",
                inner: null,
                statusCode: response.StatusCode);
        }
    }

    private string BuildPath(string relativePath)
    {
        if (relativePath.Contains("://") || relativePath.TrimStart().StartsWith("//"))
            throw new ArgumentException("Only relative paths are allowed.", nameof(relativePath));

        var path = relativePath.TrimStart('/');

        if (this.apiOptions.ApiKeyLocation == ApiKeyLocation.Query)
        {
            path = QueryHelpers.AddQueryString(
                path,
                this.apiOptions.ApiKeyName,
                this.apiOptions.ApiKey);
        }

        return path;
    }

    private static string Truncate(string text)
        => text.Length <= MaxErrorBodyLength
            ? text
            : text[..MaxErrorBodyLength] + "...";
}