using SipSavy.Data.Domain;

namespace SipSavy.Data;

public interface IQueryFacade
{
    IQueryable<Cocktail> Cocktails { get; }
    IQueryable<Video> Videos { get; }
    IQueryable<CocktailIngredient> CocktailIngredients { get; }
}