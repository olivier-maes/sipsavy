using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;

namespace SipSavy.Web.Tests.Fixtures;

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

    public async Task DisposeAsync()
    {
        await PostgresContainer.DisposeAsync();
        await DbContext.DisposeAsync();
    }

    private static async Task SeedDatabase(AppDbContext dbContext)
    {
        await dbContext.AddAsync(TestVideo.Video1);

        await dbContext.AddRangeAsync(new List<Cocktail>
        {
            TestCocktail.Cocktail1,
            TestCocktail.Cocktail2
        });
        await dbContext.SaveChangesAsync();
    }
}