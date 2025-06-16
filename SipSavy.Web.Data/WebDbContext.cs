using Microsoft.EntityFrameworkCore;
using SipSavy.Web.Data.Domain;

namespace SipSavy.Web.Data;

public sealed class WebDbContext(DbContextOptions<WebDbContext> options) : DbContext(options)
{
    public DbSet<Cocktail> Cocktails { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new CocktailEntityTypeConfig());
    }
}