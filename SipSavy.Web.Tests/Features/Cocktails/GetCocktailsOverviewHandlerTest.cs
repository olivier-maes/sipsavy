using DotNet.Testcontainers.Builders;

namespace SipSavy.Web.Tests.Features.Cocktails;

public class GetCocktailsOverviewHandlerTest
{
    [Fact]
    public async Task Handle_ShouldReturnAllCocktails()
    {
        var container = new ContainerBuilder()
            .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
            .WithPortBinding(143, 1433)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilContainerIsHealthy())
            .Build();
        
        await container.StartAsync();
        
        
    }
}