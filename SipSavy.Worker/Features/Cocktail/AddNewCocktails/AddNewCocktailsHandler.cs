using SipSavy.Core;
using SipSavy.Data.Domain;
using SipSavy.Data.Repository;

namespace SipSavy.Worker.Features.Cocktail.AddNewCocktails;

internal sealed class AddNewCocktailsHandler(ICocktailRepository repository)
    : IHandler<AddNewCocktailsRequest, AddNewCocktailsResponse>
{
    public async Task<AddNewCocktailsResponse> Handle(AddNewCocktailsRequest request,
        CancellationToken cancellationToken)
    {
        var cocktails = request.Cocktails
            .Select(c => new Data.Domain.Cocktail
            {
                Name = c.Name,
                Description = c.Description,
                VideoId = request.VideoId,
                Ingredients = c.Ingredients.Select(x => new CocktailIngredient
                {
                    Name = x.Name,
                    Quantity = x.Quantity,
                    Unit = x.Unit
                }).ToList()
            })
            .ToList();

        var addedCocktails = await repository.AddCocktails(cocktails);

        return new AddNewCocktailsResponse
        {
            Cocktails = addedCocktails.Select(c => new AddNewCocktailsResponse.CocktailDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                VideoId = c.VideoId,
                Ingredients = c.Ingredients.Select(i => new AddNewCocktailsResponse.IngredientDto
                {
                    Id = i.Id,
                    Name = i.Name,
                    Quantity = i.Quantity,
                    Unit = i.Unit
                }).ToList()
            }).ToList()
        };
    }
}