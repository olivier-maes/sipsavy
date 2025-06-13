using Pgvector;

namespace SipSavy.Worker.Data.Domain;

public class VideoChunk
{
    public int Id { get; set; }
    public required string Content { get; set; } = string.Empty;
    public required Vector Embedding { get; set; }
    public required int VideoId { get; set; }
    public Video Video { get; set; } = null!;
}