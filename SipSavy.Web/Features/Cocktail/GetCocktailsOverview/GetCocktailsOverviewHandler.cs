using Mediator;
using Microsoft.EntityFrameworkCore;
using SipSavy.Data;

namespace SipSavy.Web.Features.Cocktail.GetCocktailsOverview;

internal sealed class GetCocktailsOverviewHandler(IQueryFacade queryFacade)
    : IRequestHandler<GetCocktailsOverviewRequest, GetCocktailsOverviewResponse>
{
    public async ValueTask<GetCocktailsOverviewResponse> Handle(GetCocktailsOverviewRequest request,
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