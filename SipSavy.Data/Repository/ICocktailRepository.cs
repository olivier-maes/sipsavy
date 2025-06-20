using SipSavy.Data.Domain;

namespace SipSavy.Data.Repository;

public interface ICocktailRepository
{
    Task<Cocktail> AddCocktail(Cocktail cocktail);
    Task<List<Cocktail>> AddCocktails(List<Cocktail> cocktails);
}