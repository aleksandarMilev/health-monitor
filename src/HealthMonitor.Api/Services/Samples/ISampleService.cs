namespace HealthMonitor.Api.Services.Samples;

using Models.Dtos;

public interface ISampleService
{
    Task<IReadOnlyList<SampleResponseDto>> GetAll(
        string? name,
        DateTime? from,
        DateTime? to,
        CancellationToken cancellationToken = default);

    Task<SampleResponseDto?> GetById(
        int id,
        CancellationToken cancellationToken = default);

    Task<SampleResponseDto> Create(
        SampleCreateDto dto,
        CancellationToken cancellationToken = default);

    Task<SampleResponseDto?> Update(
        int id,
        SampleCreateDto dto,
        CancellationToken cancellationToken = default);

    Task<bool> Delete(
        int id,
        CancellationToken cancellationToken = default);
}
