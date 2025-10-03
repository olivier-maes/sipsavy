using SipSavy.Web.Features.Cocktail.GetCocktailDetail;
using SipSavy.Web.Tests.Fixtures;

namespace SipSavy.Web.Tests.Features.Cocktails;

public class GetCocktailDetailHandlerTest(PostgresFixture fixture) : IClassFixture<PostgresFixture>
{
    [Fact]
    public async Task Handle_ShouldReturnCocktailDetailsById()
    {
        // Arrange
        var queryFacade = new QueryFacade(fixture.DbContext);
        var expected = new GetCocktailDetailResponse
        {
            Cocktail = new GetCocktailDetailResponse.CocktailDto
            {
                Id = TestCocktail.Cocktail1.Id,
                Name = TestCocktail.Cocktail1.Name,
                Description = TestCocktail.Cocktail1.Description,
                Ingredients = TestCocktail.Cocktail1.Ingredients.Select(x => new GetCocktailDetailResponse.IngredientDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Quantity = x.Quantity,
                    Unit = x.Unit
                }).ToList(),
                VideoId = TestCocktail.Cocktail1.VideoId
            }
        };

        // Act
        var sut = new GetCocktailDetailHandler(queryFacade);
        var response =
            await sut.Handle(new GetCocktailDetailRequest(TestCocktail.Cocktail1.Id), CancellationToken.None);

        // Assert
        Assert.IsType<GetCocktailDetailResponse>(response);
        Assert.Equivalent(expected, response);
    }
}