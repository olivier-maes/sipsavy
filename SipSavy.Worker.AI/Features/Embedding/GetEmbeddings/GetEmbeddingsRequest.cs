namespace SipSavy.Worker.AI.Features.Embedding.GetEmbeddings;

public sealed record GetEmbeddingsRequest(List<string> Texts, string Model);