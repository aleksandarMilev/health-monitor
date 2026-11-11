namespace HealthMonitor.Api.Options;

public class ExternalApiOptions
{
    public const string SectionName = "ExternalApi";

    public string BaseUrl { get; set; } = "";

    public ApiKeyLocation ApiKeyLocation { get; set; } = ApiKeyLocation.None;

    public string ApiKeyName { get; set; } = "";

    public string ApiKey { get; set; } = "";

    public int TimeoutSeconds { get; set; } = 15;

    public string? GetConfigurationError()
    {
        if (string.IsNullOrWhiteSpace(this.BaseUrl))
            return "ExternalApi:BaseUrl is empty.";

        if (!Uri.TryCreate(this.BaseUrl, UriKind.Absolute, out var uri) ||
            (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
            return $"ExternalApi:BaseUrl '{this.BaseUrl}' is not a valid http(s) URL.";

        if (this.ApiKeyLocation != ApiKeyLocation.None && string.IsNullOrWhiteSpace(this.ApiKey))
            return $"ExternalApi:ApiKeyLocation is '{this.ApiKeyLocation}' but ExternalApi:ApiKey is empty. Set the key, or set ApiKeyLocation to 'None'.";

        if (this.ApiKeyLocation is ApiKeyLocation.Query or ApiKeyLocation.Header &&
            string.IsNullOrWhiteSpace(this.ApiKeyName))
            return $"ExternalApi:ApiKeyLocation is '{this.ApiKeyLocation}' but ExternalApi:ApiKeyName is empty (e.g. 'api_key' or 'X-Api-Key').";

        if (this.TimeoutSeconds <= 0)
            return "ExternalApi:TimeoutSeconds must be greater than 0.";

        return null;
    }
}