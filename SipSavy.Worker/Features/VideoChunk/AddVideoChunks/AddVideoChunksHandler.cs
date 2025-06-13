using Pgvector;
using SipSavy.Core;
using SipSavy.Worker.Data;

namespace SipSavy.Worker.Features.VideoChunk.AddVideoChunks;

internal sealed class AddVideoChunksHandler(IVectorStore vectorStore)
    : IHandler<AddVideoChunksRequest, AddVideoChunksResponse>
{
    public async Task<AddVideoChunksResponse> Handle(AddVideoChunksRequest request)
    {
        await vectorStore.StoreChunksAsync(request.VideoChunks.Select(x => new Data.Domain.VideoChunk
        {
            VideoId = x.VideoId,
            Content = x.Content,
            Embedding = new Vector(x.Embedding)
        }));

        return new AddVideoChunksResponse();
    }
}