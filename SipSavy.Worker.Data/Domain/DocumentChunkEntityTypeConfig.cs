using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SipSavy.Worker.Data.Domain;

public sealed class DocumentChunkEntityTypeConfig : IEntityTypeConfiguration<DocumentChunk>
{
    public void Configure(EntityTypeBuilder<DocumentChunk> builder)
    {
        builder.ToTable("document_chunks");

        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.Video);

        builder.Property(x => x.Embedding)
            .HasColumnType("vector(1536)");

        builder.HasIndex(x => x.Embedding)
            .HasMethod("ivfflat")
            .HasOperators("vector_cosine_ops");
    }
}