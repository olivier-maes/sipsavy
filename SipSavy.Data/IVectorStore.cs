using Pgvector;
using SipSavy.Data.Domain;

namespace SipSavy.Data;

public interface IVectorStore
{
    Task StoreChunksAsync(IEnumerable<VideoChunk> documentChunks);
    Task<List<VideoChunk>?> SearchAsync(Vector queryVector, int topK = 5, double threshold = 0.7);
}