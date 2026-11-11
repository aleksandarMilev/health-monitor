namespace HealthMonitor.Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using Models.Dtos;
using Services.Samples;

[ApiController]
[Route("api/[controller]")]
public class SamplesController(
    ISampleService samples) : ControllerBase
{
    [HttpGet]
    // used by Scalar for better documentation, not required.
    [ProducesResponseType<IReadOnlyList<SampleResponseDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<SampleResponseDto>>> GetAll(
        string? name,
        DateTime? from,
        DateTime? to,
        CancellationToken cancellationToken)
    {
        var result = await samples.GetAll(
            name,
            from,
            to,
            cancellationToken);

        return this.Ok(result);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType<SampleResponseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SampleResponseDto>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var sample = await samples.GetById(
            id,
            cancellationToken);

        return sample is null
            ? this.NotFound()
            : this.Ok(sample);
    }

    [HttpPost]
    [ProducesResponseType<SampleResponseDto>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<SampleResponseDto>> Create(
        SampleCreateDto dto,
        CancellationToken cancellationToken)
    {
        var created = await samples.Create(
            dto,
            cancellationToken);

        return this.CreatedAtAction(
            nameof(GetById),
            new { id = created.Id },
            created);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType<SampleResponseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SampleResponseDto>> Update(
        int id,
        SampleCreateDto dto,
        CancellationToken cancellationToken)
    {
        var updated = await samples.Update(
            id,
            dto,
            cancellationToken);

        return updated is null
            ? this.NotFound()
            : this.Ok(updated);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        int id,
        CancellationToken cancellationToken)
    {
        var deleted = await samples.Delete(
            id,
            cancellationToken);

        return deleted
            ? this.NoContent()
            : this.NotFound();
    }
}
