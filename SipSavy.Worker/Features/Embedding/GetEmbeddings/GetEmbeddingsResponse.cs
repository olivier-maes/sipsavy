namespace SipSavy.Worker.Features.Embedding.GetEmbeddings;

public sealed record GetEmbeddingsResponse
{
    public float[] Embedding { get; set; } = [];
}