namespace SipSavy.Worker.Data.Domain;

public class DocumentChunk
{
    public int Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public float[] Embedding { get; set; } = [];

    public int VideoId { get; set; }
    public Video Video { get; set; } = null!;
}