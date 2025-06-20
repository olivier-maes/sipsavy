using Microsoft.EntityFrameworkCore;
using SipSavy.Core;
using SipSavy.Web.Data;

namespace SipSavy.Web.Features.Cocktail.GetCocktailsOverview;

internal sealed class GetCocktailsOverviewHandler(IQueryFacade queryFacade)
    : IHandler<GetCocktailsOverviewRequest, GetCocktailsOverviewResponse>
{
    public async Task<GetCocktailsOverviewResponse> Handle(GetCocktailsOverviewRequest request,
        CancellationToken cancellationToken)
    {
        return new GetCocktailsOverviewResponse
        {
            Cocktails = await queryFacade.Cocktails.Select(x => new GetCocktailsOverviewResponse.CocktailDto
            {
                Id = x.Id,
                Name = x.Name
            }).ToListAsync(cancellationToken)
        };
    }
}