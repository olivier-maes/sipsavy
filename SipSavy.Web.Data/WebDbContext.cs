using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using SipSavy.Web.Data.Domain;

namespace SipSavy.Web.Data;

public sealed class WebDbContext : DbContext
{
    public WebDbContext(DbContextOptions<WebDbContext> options) : base(options)
    {
        Debug.WriteLine($"{ContextId} context created.");
    }

    public DbSet<Cocktail> Cocktails { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new CocktailEntityTypeConfig());
    }
}