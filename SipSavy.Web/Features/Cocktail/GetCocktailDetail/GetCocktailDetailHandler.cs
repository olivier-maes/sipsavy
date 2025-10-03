using Mediator;
using Microsoft.EntityFrameworkCore;
using SipSavy.Data;

namespace SipSavy.Web.Features.Cocktail.GetCocktailDetail;

internal sealed class GetCocktailDetailHandler(IQueryFacade queryFacade)
    : IRequestHandler<GetCocktailDetailRequest, GetCocktailDetailResponse>
{
    public async ValueTask<GetCocktailDetailResponse> Handle(GetCocktailDetailRequest request,
        CancellationToken cancellationToken)
    {
        var cocktail = queryFacade.Cocktails
            .Include(x => x.Ingredients)
            .SingleOrDefault(x => x.Id == request.Id);

        if (cocktail == null)
        {
            return new GetCocktailDetailResponse();
        }

        return new GetCocktailDetailResponse
        {
            Cocktail = new GetCocktailDetailResponse.CocktailDto
            {
                Id = cocktail.Id,
                Name = cocktail.Name,
                Description = cocktail.Description,
                Ingredients = cocktail.Ingredients.Select(x => new GetCocktailDetailResponse.IngredientDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Quantity = x.Quantity,
                    Unit = x.Unit
                }).ToList(),
                VideoId = cocktail.VideoId
            }
        };
    }
}