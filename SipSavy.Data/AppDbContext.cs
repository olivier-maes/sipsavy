using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using SipSavy.Data.Domain;

namespace SipSavy.Data;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
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