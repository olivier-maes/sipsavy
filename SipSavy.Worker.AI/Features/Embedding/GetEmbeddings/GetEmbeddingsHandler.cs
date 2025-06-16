using OllamaSharp;
using SipSavy.Core;

namespace SipSavy.Worker.AI.Features.Embedding.GetEmbeddings;

public sealed class GetEmbeddingsHandler(IOllamaApiClient ollamaApiClient)
    : IHandler<GetEmbeddingsRequest, GetEmbeddingsResponse>
{
    public async Task<GetEmbeddingsResponse> Handle(GetEmbeddingsRequest request)
    {
        var embeddings = await ollamaApiClient.EmbedAsync(request.Text);

        return new GetEmbeddingsResponse
        {
            Embedding = embeddings.Embeddings.First()
        };
    }
}