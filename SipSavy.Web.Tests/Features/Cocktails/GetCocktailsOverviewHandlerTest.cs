using SipSavy.Data.Migrations;
using SipSavy.Web.Features.Cocktail.GetCocktailsOverview;

namespace SipSavy.Web.Tests.Features.Cocktails;

public class GetCocktailsOverviewHandlerTest : IClassFixture<MsSqlFixture>
{
    [Fact]
    public async Task Handle_ShouldReturnAllCocktails()
    {
        // Arrange
        var dbContext = await MsSqlFixture.GetDbContext();
        var queryFacade = new QueryFacade(dbContext);

        // Act
        var sut = new GetCocktailsOverviewHandler(queryFacade);
        var response = await sut.Handle(new GetCocktailsOverviewRequest());

        // Assert
        Assert.IsType<GetCocktailsOverviewResponse>(response);
        Assert.Equal(2, response.Cocktails.Count);
        var cocktail1 = response.Cocktails.FirstOrDefault(c => c.Name == TestCocktail.Cocktail1.Name);
        Assert.Equal(cocktail1?.Name, TestCocktail.Cocktail1.Name);
        var cocktail2 = response.Cocktails.FirstOrDefault(c => c.Name == TestCocktail.Cocktail1.Name);
        Assert.Equal(cocktail2?.Name, TestCocktail.Cocktail1.Name);
    }
}