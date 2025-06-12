namespace SipSavy.Worker.AI.Features.Embedding.GetEmbeddings;

public sealed record GetEmbeddingsResponse
{
    public List<float[]> Embeddings { get; set; } = [];
}