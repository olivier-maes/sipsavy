using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SipSavy.Data.Domain;

public sealed class CocktailEntityTypeConfig : IEntityTypeConfiguration<Cocktail>
{
    public void Configure(EntityTypeBuilder<Cocktail> builder)
    {
        builder.ToTable("Cocktails");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(250);
    }
}