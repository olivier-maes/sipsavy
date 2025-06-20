using Microsoft.EntityFrameworkCore;
using SipSavy.Data.Domain;

namespace SipSavy.Data;

public sealed class QueryFacade(AppDbContext dbContext) : IQueryFacade
{
    public IQueryable<Cocktail> Cocktails => dbContext.Cocktails.AsQueryable().AsNoTracking();
    public IQueryable<Video> Videos => dbContext.Videos.AsQueryable().AsNoTracking();

    public IQueryable<CocktailIngredient> CocktailIngredients =>
        dbContext.CocktailIngredients.AsQueryable().AsNoTracking();
}