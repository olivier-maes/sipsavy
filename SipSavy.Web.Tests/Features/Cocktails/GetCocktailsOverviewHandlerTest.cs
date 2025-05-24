using SipSavy.Data.Domain;
using SipSavy.Data.Migrations;
using SipSavy.Web.Features.Cocktail.GetCocktailsOverview;
using SipSavy.Web.Tests.Fixtures;

namespace SipSavy.Web.Tests.Features.Cocktails;

public class GetCocktailsOverviewHandlerTest : IClassFixture<MsSqlFixture>
{
    private readonly Cocktail _cocktail1 = new Cocktail
    {
        Name = "Cocktail 1",
    };

    private readonly Cocktail _cocktail2 = new Cocktail
    {
        Name = "Cocktail 2",
    };

    [Fact]
    public async Task Handle_ShouldReturnAllCocktails()
    {
        // Arrange
        var dbContext = MsSqlFixture.GetDbContext();
        var queryFacade = new QueryFacade(dbContext);
        await dbContext.AddRangeAsync(new List<Cocktail>
        {
            _cocktail1,
            _cocktail2
        });
        await dbContext.SaveChangesAsync();

        // Act
        var sut = new GetCocktailsOverviewHandler(queryFacade);
        var response = await sut.Handle(new GetCocktailsOverviewRequest());

        // Assert
        Assert.IsType<GetCocktailsOverviewResponse>(response);
        Assert.Equal(2, response.Cocktails.Count);
        var cocktail1 = response.Cocktails.FirstOrDefault(c => c.Name == _cocktail1.Name);
        Assert.Equal(cocktail1?.Name, _cocktail1.Name);
        var cocktail2 = response.Cocktails.FirstOrDefault(c => c.Name == _cocktail2.Name);
        Assert.Equal(cocktail2?.Name, _cocktail2.Name);
    }
}