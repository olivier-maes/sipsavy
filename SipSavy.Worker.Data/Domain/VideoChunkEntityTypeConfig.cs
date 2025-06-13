using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SipSavy.Worker.Data.Domain;

public sealed class VideoChunkEntityTypeConfig : IEntityTypeConfiguration<VideoChunk>
{
    public void Configure(EntityTypeBuilder<VideoChunk> builder)
    {
        builder.ToTable("video_chunks");

        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.Video)
            .WithMany(x => x.Chunks)
            .HasForeignKey(x => x.VideoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.VideoId);

        builder.Property(x => x.Embedding)
            .HasColumnType("vector(384)");

        builder.Property(x => x.Content)
            .IsRequired();
        
        builder.HasIndex(e => e.Embedding)
            .HasMethod("ivfflat")
            .HasOperators("vector_cosine_ops");
    }
}