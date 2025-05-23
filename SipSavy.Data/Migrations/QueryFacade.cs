using Microsoft.EntityFrameworkCore;
using SipSavy.Data.Domain;

namespace SipSavy.Data.Migrations;

public sealed class QueryFacade(AppDbContext dbContext) : IQueryFacade
{
    public IQueryable<Cocktail> Cocktails => dbContext.Cocktails.AsQueryable().AsNoTracking();
}