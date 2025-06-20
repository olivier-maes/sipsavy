using Microsoft.EntityFrameworkCore;
using SipSavy.Data;
using SipSavy.Data.Domain;
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

    public static async Task<AppDbContext> GetDbContext()
    {
        var dbContext = new AppDbContext(new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(PostgresContainer.GetConnectionString())
            .Options);

        await dbContext.Database.MigrateAsync();
        await SeedDatabase(dbContext);

        return dbContext;
    }

    private static async Task SeedDatabase(AppDbContext dbContext)
    {
        await dbContext.AddRangeAsync(new List<Cocktail>
        {
            TestCocktail.Cocktail1,
            TestCocktail.Cocktail2
        });
        await dbContext.SaveChangesAsync();
    }
}