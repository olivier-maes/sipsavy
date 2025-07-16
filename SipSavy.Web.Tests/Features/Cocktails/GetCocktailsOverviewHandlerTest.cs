using SipSavy.Web.Features.Cocktail.GetCocktailsOverview;
using SipSavy.Web.Tests.Fixtures;

namespace SipSavy.Web.Tests.Features.Cocktails;

public class GetCocktailsOverviewHandlerTest(PostgresFixture fixture) : IClassFixture<PostgresFixture>
{
    [Fact]
    public async Task Handle_ShouldReturnAllCocktails()
    {
        // Arrange
        var queryFacade = new QueryFacade(fixture.DbContext);

        // Act
        var sut = new GetCocktailsOverviewHandler(queryFacade);
        var response = await sut.Handle(new GetCocktailsOverviewRequest(), CancellationToken.None);

        // Assert
        Assert.IsType<GetCocktailsOverviewResponse>(response);
        Assert.Equal(2, response.Cocktails.Count);
        var cocktail1 = response.Cocktails.FirstOrDefault(c => c.Name == TestCocktail.Cocktail1.Name);
        Assert.Equal(cocktail1?.Name, TestCocktail.Cocktail1.Name);
        var cocktail2 = response.Cocktails.FirstOrDefault(c => c.Name == TestCocktail.Cocktail2.Name);
        Assert.Equal(cocktail2?.Name, TestCocktail.Cocktail2.Name);
    }
}