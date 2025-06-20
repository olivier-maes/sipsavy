using Microsoft.EntityFrameworkCore;
using Pgvector;
using Pgvector.EntityFrameworkCore;
using SipSavy.Data.Domain;

namespace SipSavy.Data;

public sealed class PostgresVectorStore(AppDbContext dbContext) : IVectorStore
{
    public async Task StoreChunksAsync(IEnumerable<VideoChunk> documentChunks)
    {
        dbContext.AddRange(documentChunks);
        await dbContext.SaveChangesAsync();
    }

    public async Task<List<VideoChunk>?> SearchAsync(Vector queryVector, int topK = 5, double threshold = 0.7)
    {
        var results = await dbContext.VideoChunks
            .OrderBy(x => x.Embedding.CosineDistance(queryVector))
            .Take(topK)
            .Select(x => new
            {
                Chunk = x,
                Distance = x.Embedding.CosineDistance(queryVector)
            }).ToListAsync();

        return results
            .Where(r => r.Distance <= 2 - threshold)
            .Select(r => new VideoChunk
            {
                Id = r.Chunk.Id,
                VideoId = r.Chunk.VideoId,
                Content = r.Chunk.Content,
                Embedding = r.Chunk.Embedding,
            })
            .ToList();
    }
}