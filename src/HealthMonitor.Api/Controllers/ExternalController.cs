namespace HealthMonitor.Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using Services.External;

[ApiController]
[Route("api/[controller]")]
public class ExternalController(
    IExternalApiClient externalApi) : ControllerBase
{
    /// <param name="path">Relative path on the external API, e.g. "planetary/apod".</param>
    [HttpGet]
    // used by Scalar for better documentation, not required.
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status502BadGateway)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    [ProducesResponseType(StatusCodes.Status504GatewayTimeout)]
    public async Task<IActionResult> Get(
        string path,
        CancellationToken cancellationToken)
    {
        if (!externalApi.IsConfigured)
        {
            return this.Problem(
                title: "External API is not configured",
                detail: externalApi.ConfigurationError,
                statusCode: StatusCodes.Status503ServiceUnavailable);
        }

        try
        {
            var json = await externalApi.GetRaw(
                path,
                cancellationToken);

            return this.Content(json, "application/json");
        }
        catch (ArgumentException exception)
        {
            return this.Problem(
                title: "Invalid path",
                detail: exception.Message,
                statusCode: StatusCodes.Status400BadRequest);
        }
        catch (HttpRequestException exception)
        {
            return this.Problem(
                title: "External API request failed",
                detail: exception.Message,
                statusCode: StatusCodes.Status502BadGateway);
        }
        catch (TaskCanceledException exception)
            when (exception.InnerException is TimeoutException)
        {
            return this.Problem(
                title: "External API timed out",
                detail: "No response within ExternalApi:TimeoutSeconds.",
                statusCode: StatusCodes.Status504GatewayTimeout);
        }
    }
}
