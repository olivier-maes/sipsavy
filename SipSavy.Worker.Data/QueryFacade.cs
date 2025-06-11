using Microsoft.EntityFrameworkCore;
using SipSavy.Worker.Data.Domain;

namespace SipSavy.Worker.Data;

public sealed class QueryFacade(WorkerDbContext dbContext) : IQueryFacade
{
    public IQueryable<Video> Videos => dbContext.Videos.AsQueryable().AsNoTracking();
}