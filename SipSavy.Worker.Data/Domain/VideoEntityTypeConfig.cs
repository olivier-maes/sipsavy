using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SipSavy.Worker.Data.Domain;

public sealed class VideoEntityTypeConfig : IEntityTypeConfiguration<Video>
{
    public void Configure(EntityTypeBuilder<Video> builder)
    {
        builder.ToTable("videos");

        builder.HasKey(v => v.Id);

        builder.Property(v => v.YoutubeId)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(v => v.Title)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(v => v.Status)
            .HasDefaultValue(Status.New)
            .IsRequired()
            .HasConversion<string>();
    }
}