using Mediator;

namespace SipSavy.Worker.Features.Embedding.GetEmbeddings;

public sealed record GetEmbeddingsRequest(string Text) : IRequest<GetEmbeddingsResponse>;