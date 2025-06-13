using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SipSavy.Worker.Data.Domain;

public sealed class VideoChunkEntityTypeConfig : IEntityTypeConfiguration<VideoChunk>
{
    public void Configure(EntityTypeBuilder<VideoChunk> builder)
    {
        builder.ToTable("document_chunks");

        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.Video)
            .WithMany(x => x.Chunks)
            .HasForeignKey(x => x.VideoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.VideoId);

        builder.Property(x => x.Embedding)
            .HasColumnType("vector(768)")
            .IsRequired();

        builder.Property(x => x.Content)
            .HasMaxLength(5000)
            .IsRequired();

        builder.HasIndex(x => x.Embedding)
            .HasMethod("ivfflat")
            .HasOperators("vector_cosine_ops");
    }
}