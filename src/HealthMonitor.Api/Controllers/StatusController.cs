namespace HealthMonitor.Api.Controllers;

using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class StatusController(
    IWebHostEnvironment env) : ControllerBase
{
    [HttpGet]
    // used by Scalar for better documentation, not required.
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Get()
        => this.Ok(new
        {
            status = "ok",
            environment = env.EnvironmentName,
            serverTimeUtc = DateTime.UtcNow,
        });
}
