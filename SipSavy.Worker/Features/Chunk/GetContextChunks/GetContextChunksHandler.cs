using OllamaSharp;
using Pgvector;
using SipSavy.Core;
using SipSavy.Data;

namespace SipSavy.Worker.Features.Chunk.GetContextChunks;

internal sealed class GetContextChunksHandler : IHandler<GetContextChunksRequest, GetContextChunksResponse>
{
    private readonly IOllamaApiClient _ollamaApiClient;
    private readonly IVectorStore _vectorStore;

    public GetContextChunksHandler(IOllamaApiClient ollamaApiClient, IVectorStore vectorStore)
    {
        _ollamaApiClient = ollamaApiClient;
        _vectorStore = vectorStore;
    }

    public async Task<GetContextChunksResponse> Handle(GetContextChunksRequest request,
        CancellationToken cancellationToken)
    {
        _ollamaApiClient.SelectedModel = Environment.GetEnvironmentVariable("AI_EMBEDDING_MODEL") ??
                                         throw new Exception("AI_EMBEDDING_MODEL environment variable is not set.");

        var embedding = await _ollamaApiClient.EmbedAsync(request.Transcript, CancellationToken.None);

        var chunks = await _vectorStore.SearchAsync(new Vector(embedding.Embeddings[0]));
        if (chunks is null)
        {
            return new GetContextChunksResponse();
        }

        return new GetContextChunksResponse
        {
            Context = string.Join("\n", chunks.Select(c => c.Content))
        };
    }
}