namespace SipSavy.Worker.AI.Features.Embedding.GetEmbeddings;

public sealed record GetEmbeddingsResponse
{
    public List<float[]> Embedding { get; set; } = [];
}