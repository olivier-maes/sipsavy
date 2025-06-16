namespace SipSavy.Worker.Features.VideoChunk.AddVideoChunks;

internal sealed record AddVideoChunksRequest
{
    public List<VideoChunkDto> VideoChunks { get; init; } = [];

    internal sealed record VideoChunkDto
    {
        public int VideoId { get; init; }
        public string Content { get; init; } = string.Empty;
        public float[] Embedding { get; init; } = [];
    }
};