using Microsoft.EntityFrameworkCore;
using SipSavy.Data;
using SipSavy.Data.Domain;
using SipSavy.Worker.Tests.TestData;
using Testcontainers.PostgreSql;

namespace SipSavy.Worker.Tests.Fixtures;

public sealed class PostgresFixture : IAsyncLifetime
{
    private static readonly PostgreSqlContainer PostgresContainer = new PostgreSqlBuilder().Build();
    public AppDbContext DbContext { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        await PostgresContainer.StartAsync();

        DbContext = new AppDbContext(new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(PostgresContainer.GetConnectionString())
            .Options);

        await DbContext.Database.MigrateAsync();
        await SeedDatabase(DbContext);
    }

    public async Task ClearAndReseedAsync()
    {
        await ClearAllTables();
        await SeedDatabase(DbContext);
    }

    private async Task ClearAllTables()
    {
        DbContext.ChangeTracker.Clear();
        DbContext.RemoveRange(DbContext.Set<Video>());
        await DbContext.SaveChangesAsync();
    }


    public async Task DisposeAsync()
    {
        await PostgresContainer.DisposeAsync();
        await DbContext.DisposeAsync();
    }

    private static async Task SeedDatabase(AppDbContext dbContext)
    {
        await dbContext.AddAsync(TestVideo.Video1);

        await dbContext.AddRangeAsync(new List<Video>
        {
            TestVideo.Video1,
            TestVideo.Video2
        });
        await dbContext.SaveChangesAsync();
    }
}