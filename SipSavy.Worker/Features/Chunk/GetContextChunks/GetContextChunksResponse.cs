namespace SipSavy.Worker.Features.Chunk.GetContextChunks;

internal sealed record GetContextChunksResponse
{
    public string Context { get; init; } = string.Empty;
};