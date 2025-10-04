using Mediator;

namespace SipSavy.Web.Features.Cocktail.GetCocktailDetail;

internal sealed record GetCocktailDetailRequest(int Id) : IRequest<GetCocktailDetailResponse>;