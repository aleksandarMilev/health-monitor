namespace HealthMonitor.Api.Data;

using Microsoft.EntityFrameworkCore;
using Models.Entities;

public class AppDbContext(
    DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Sample> Samples { get; init; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
        => modelBuilder
            .Entity<Sample>(entity =>
            {
                entity
                    .HasKey(s => s.Id);

                entity
                    .Property(s => s.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity
                    .Property(s => s.Unit)
                    .HasMaxLength(20);
            });
}
