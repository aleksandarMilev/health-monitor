namespace HealthMonitor.Api.Services.Samples;

using Data;
using Microsoft.EntityFrameworkCore;
using Models.Dtos;
using Models.Entities;

public class SampleService(AppDbContext data) : ISampleService
{
    public async Task<IReadOnlyList<SampleResponseDto>> GetAll(
        string? name,
        DateTime? from,
        DateTime? to,
        CancellationToken cancellationToken = default)
    {
        var query = data
            .Samples
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(name))
            query = query.Where(s => s.Name == name);

        if (from.HasValue)
            query = query.Where(s => s.RecordedAt >= from.Value);

        if (to.HasValue)
            query = query.Where(s => s.RecordedAt <= to.Value);

        return await query
            .OrderBy(s => s.RecordedAt)
            .Select(s => ToDto(s))
            .ToListAsync(cancellationToken);
    }

    public async Task<SampleResponseDto?> GetById(
        int id,
        CancellationToken cancellationToken = default)
        => await data
            .Samples
            .Select(s => ToDto(s))
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

    public async Task<SampleResponseDto> Create(
        SampleCreateDto dto,
        CancellationToken cancellationToken = default)
    {
        var sample = new Sample
        {
            Name = dto.Name.Trim(),
            Value = dto.Value!.Value,
            Unit = dto.Unit?.Trim(),
            RecordedAt = dto.RecordedAt?.ToUniversalTime() ?? DateTime.UtcNow,
        };

        data.Add(sample);
        await data.SaveChangesAsync(cancellationToken);

        return ToDto(sample);
    }

    public async Task<SampleResponseDto?> Update(
        int id,
        SampleCreateDto dto,
        CancellationToken cancellationToken = default)
    {
        var sample = await data
            .Samples
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

        if (sample is null)
            return null;

        sample.Name = dto.Name.Trim();
        sample.Value = dto.Value!.Value;
        sample.Unit = dto.Unit?.Trim();
        sample.RecordedAt = dto.RecordedAt?.ToUniversalTime() ?? sample.RecordedAt;

        await data.SaveChangesAsync(cancellationToken);
        return ToDto(sample);
    }

    public async Task<bool> Delete(
        int id,
        CancellationToken cancellationToken = default)
    {
        var sample = await data
            .Samples
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

        if (sample is null)
            return false;

        data.Remove(sample);
        await data.SaveChangesAsync(cancellationToken);

        return true;
    }

    private static SampleResponseDto ToDto(Sample sample)
        => new()
        {
            Id = sample.Id,
            Name = sample.Name,
            Value = sample.Value,
            Unit = sample.Unit,
            RecordedAt = sample.RecordedAt,
        };
}
