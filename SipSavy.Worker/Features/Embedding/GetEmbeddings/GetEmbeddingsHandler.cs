using Mediator;
using OllamaSharp;

namespace SipSavy.Worker.Features.Embedding.GetEmbeddings;

public sealed class GetEmbeddingsHandler
    : IRequestHandler<GetEmbeddingsRequest, GetEmbeddingsResponse>
{
    private readonly IOllamaApiClient _ollamaApiClient;

    public GetEmbeddingsHandler(IOllamaApiClient ollamaApiClient)
    {
        _ollamaApiClient = ollamaApiClient;
    }

    public async ValueTask<GetEmbeddingsResponse> Handle(GetEmbeddingsRequest request, CancellationToken cancellationToken)
    {
        _ollamaApiClient.SelectedModel = Environment.GetEnvironmentVariable("AI_EMBEDDING_MODEL") ??
                                         throw new Exception("AI_EMBEDDING_MODEL not found");

        var embeddings = await _ollamaApiClient.EmbedAsync(request.Text, cancellationToken);

        return new GetEmbeddingsResponse
        {
            Embedding = embeddings.Embeddings.First()
        };
    }
}