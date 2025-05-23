using Microsoft.EntityFrameworkCore;

namespace SipSavy.Web.Infrastructure.Relational;

internal sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions options) : base(options)
    {
    }
}