using Microsoft.EntityFrameworkCore;
using SipSavy.Data.Domain;
using SipSavy.Data.Domain.EntityTypeConfig;

namespace SipSavy.Data;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Cocktail> Cocktails { get; set; }
    public DbSet<Video> Videos { get; set; }
    public DbSet<CocktailIngredient> CocktailIngredients { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new VideoEntityTypeConfig());
        modelBuilder.ApplyConfiguration(new CocktailEntityTypeConfig());
        modelBuilder.ApplyConfiguration(new CocktailIngredientEntityTypeConfig());

        base.OnModelCreating(modelBuilder);
    }
}