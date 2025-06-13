using SipSavy.Worker.Data.Domain;

namespace SipSavy.Worker.Data;

using Microsoft.EntityFrameworkCore;

public sealed class WorkerDbContext(DbContextOptions<WorkerDbContext> options) : DbContext(options)
{
    public DbSet<Video> Videos { get; set; }
    public DbSet<VideoChunk> VideoChunks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("vector");

        modelBuilder.ApplyConfiguration(new VideoEntityTypeConfig());
        modelBuilder.ApplyConfiguration(new VideoChunkEntityTypeConfig());

        base.OnModelCreating(modelBuilder);
    }
}