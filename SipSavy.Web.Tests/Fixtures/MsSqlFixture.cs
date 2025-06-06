using Microsoft.EntityFrameworkCore;
using SipSavy.Data;
using Testcontainers.MsSql;

namespace SipSavy.Web.Tests.Fixtures;

public sealed class MsSqlFixture : IAsyncLifetime
{
    private static readonly MsSqlContainer MsSqlContainer = new MsSqlBuilder().Build();

    public Task InitializeAsync()
    {
        return MsSqlContainer.StartAsync();
    }

    public Task DisposeAsync()
    {
        return MsSqlContainer.DisposeAsync().AsTask();
    }

    public static async Task<AppDbContext> GetDbContext()
    {
        var dbContext = new AppDbContext(new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer(MsSqlContainer.GetConnectionString())
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