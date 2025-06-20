using Pgvector;

namespace SipSavy.Worker.Data.Domain;

public class VideoChunk
{
    public int Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public Vector Embedding { get; set; }
    public int VideoId { get; set; }
    public Video Video { get; set; }
}