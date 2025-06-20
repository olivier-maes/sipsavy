using SipSavy.Data.Domain;

namespace SipSavy.Data.Repository;

public class CocktailRepository(AppDbContext dbContext) : ICocktailRepository
{
    public async Task<Cocktail> AddCocktail(Cocktail cocktail)
    {
        await dbContext.Cocktails.AddAsync(cocktail);
        await dbContext.SaveChangesAsync();
        return cocktail;
    }

    public async Task<List<Cocktail>> AddCocktails(List<Cocktail> cocktails)
    {
        await dbContext.Cocktails.AddRangeAsync(cocktails);
        await dbContext.SaveChangesAsync();
        return cocktails;
    }
}