using OllamaSharp;
using SipSavy.Core;

namespace SipSavy.Worker.AI.Features.Embedding.GetEmbeddings;

public sealed class GetEmbeddingsHandler
    : IHandler<GetEmbeddingsRequest, GetEmbeddingsResponse>
{
    private readonly IOllamaApiClient _ollamaApiClient;

    public GetEmbeddingsHandler(IOllamaApiClient ollamaApiClient)
    {
        _ollamaApiClient = ollamaApiClient;
        _ollamaApiClient.SelectedModel = "all-minilm";
    }

    public async Task<GetEmbeddingsResponse> Handle(GetEmbeddingsRequest request, CancellationToken cancellationToken)
    {
        var embeddings = await _ollamaApiClient.EmbedAsync(request.Text, cancellationToken);

        return new GetEmbeddingsResponse
        {
            Embedding = embeddings.Embeddings.First()
        };
    }
}