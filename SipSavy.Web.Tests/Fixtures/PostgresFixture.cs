using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;

namespace SipSavy.Web.Tests.Fixtures;

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

    public static async Task<WebDbContext> GetDbContext()
    {
        var dbContext = new WebDbContext(new DbContextOptionsBuilder<WebDbContext>()
            .UseNpgsql(PostgresContainer.GetConnectionString())
            .Options);

        await dbContext.Database.MigrateAsync();
        await SeedDatabase(dbContext);

        return dbContext;
    }

    private static async Task SeedDatabase(WebDbContext dbContext)
    {
        await dbContext.AddRangeAsync(new List<Cocktail>
        {
            TestCocktail.Cocktail1,
            TestCocktail.Cocktail2
        });
        await dbContext.SaveChangesAsync();
    }
}