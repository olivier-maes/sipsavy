using System.Diagnostics;
using SipSavy.Worker.Data.Domain;

namespace SipSavy.Worker.Data;

using Microsoft.EntityFrameworkCore;

public sealed class WorkerDbContext : DbContext
{
    public WorkerDbContext(DbContextOptions<WorkerDbContext> options) : base(options)
    {
        Debug.WriteLine($"{ContextId} context created.");
    }

    public DbSet<Video> Videos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        //modelBuilder.HasPostgresExtension("vector");
        modelBuilder.ApplyConfiguration(new VideoEntityTypeConfig());
    }
}