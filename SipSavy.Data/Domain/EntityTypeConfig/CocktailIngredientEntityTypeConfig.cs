using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SipSavy.Data.Domain.EntityTypeConfig;

public class CocktailIngredientEntityTypeConfig : IEntityTypeConfiguration<CocktailIngredient>
{
    public void Configure(EntityTypeBuilder<CocktailIngredient> builder)
    {
        builder.ToTable("cocktail_ingredients");

        builder.HasKey(ci => ci.Id);

        builder.Property(ci => ci.Name)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(ci => ci.Unit)
            .HasConversion<string>();

        builder.HasOne(x => x.Cocktail)
            .WithMany(c => c.Ingredients)
            .HasForeignKey(x => x.CocktailId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}