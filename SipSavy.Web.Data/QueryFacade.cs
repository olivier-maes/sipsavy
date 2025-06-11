using Microsoft.EntityFrameworkCore;
using SipSavy.Web.Data.Domain;

namespace SipSavy.Web.Data;

public sealed class QueryFacade(WebDbContext dbContext) : IQueryFacade
{
    public IQueryable<Cocktail> Cocktails => dbContext.Cocktails.AsQueryable().AsNoTracking();
}