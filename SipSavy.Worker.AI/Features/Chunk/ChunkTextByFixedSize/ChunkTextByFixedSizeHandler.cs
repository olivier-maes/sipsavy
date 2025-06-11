using SipSavy.Core;

namespace SipSavy.Worker.AI.Features.Chunk.ChunkTextByFixedSize;

public sealed class ChunkTextByFixedSizeHandler : IHandler<ChunkTextByFixedSizeRequest, ChunkTextByFixedSizeResponse>
{
    public async Task<ChunkTextByFixedSizeResponse> Handle(ChunkTextByFixedSizeRequest request)
    {
        var chunks = new List<ChunkTextByFixedSizeResponse.TextChunkDto>();

        var chunkIndex = 0;
        var start = 0;

        while (start < request.Text.Length)
        {
            var end = Math.Min(start + request.ChunkSize, request.Text.Length);
            var chunkText = request.Text.Substring(start, end - start);

            chunks.Add(new ChunkTextByFixedSizeResponse.TextChunkDto
            {
                Content = chunkText.Trim(),
                Index = chunkIndex++,
                StartPosition = start,
                EndPosition = end,
                ChunkingMethod = "FixedSize"
            });

            start = Math.Max(start + request.ChunkSize - request.Overlap, end);
        }

        return new ChunkTextByFixedSizeResponse
        {
            Chunks = chunks
        };
    }
}