using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SipSavy.Data.Domain.EntityTypeConfig;

public sealed class CocktailEntityTypeConfig : IEntityTypeConfiguration<Cocktail>
{
    public void Configure(EntityTypeBuilder<Cocktail> builder)
    {
        builder.ToTable("cocktails");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(c => c.Description)
            .HasMaxLength(5000);

        builder.HasMany(c => c.Ingredients)
            .WithOne(x => x.Cocktail)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(c => c.VideoId);

        builder.HasOne(c => c.Video)
            .WithMany(x => x.Cocktails)
            .OnDelete(DeleteBehavior.NoAction);
    }
}