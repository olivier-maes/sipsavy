using Microsoft.EntityFrameworkCore;
using SipSavy.Worker.Data;
using SipSavy.Worker.Data.Domain;
using SipSavy.Worker.Tests.TestData;
using Testcontainers.PostgreSql;

namespace SipSavy.Worker.Tests.Fixtures;

public sealed class PostgresFixture : IAsyncLifetime
{
    private static readonly PostgreSqlContainer PostgresContainer = new PostgreSqlBuilder().Build();

    public Task InitializeAsync()
    {
        return PostgresContainer.StartAsync();
    }

    public Task DisposeAsync()
    {
        return PostgresContainer.DisposeAsync().AsTask();
    }

    public static async Task<WorkerDbContext> GetDbContext()
    {
        var dbContext = new WorkerDbContext(new DbContextOptionsBuilder<WorkerDbContext>()
            .UseNpgsql(PostgresContainer.GetConnectionString())
            .Options);

        await dbContext.Database.MigrateAsync();
        await SeedDatabase(dbContext);

        return dbContext;
    }

    private static async Task SeedDatabase(WorkerDbContext dbContext)
    {
        await dbContext.AddRangeAsync(new List<Video>
        {
            TestVideo.Video1,
            TestVideo.Video2
        });
        await dbContext.SaveChangesAsync();
    }
}