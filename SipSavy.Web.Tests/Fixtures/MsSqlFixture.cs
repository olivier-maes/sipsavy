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

    public static AppDbContext GetDbContext()
    {
        var dbContext = new AppDbContext(new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer(MsSqlContainer.GetConnectionString())
            .Options);

        dbContext.Database.Migrate();

        return dbContext;
    }
}