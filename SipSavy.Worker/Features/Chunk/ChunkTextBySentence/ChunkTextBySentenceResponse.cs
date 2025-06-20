namespace SipSavy.Worker.Features.Chunk.ChunkTextBySentence;

public sealed record ChunkTextBySentenceResponse
{
    public List<TextChunkDto> Chunks { get; set; } = [];

    public sealed record TextChunkDto
    {
        public string Content { get; set; } = string.Empty;
        public int Index { get; set; }
        public int StartPosition { get; set; }
        public int EndPosition { get; set; }
        public string ChunkingMethod { get; set; } = string.Empty;
        public Dictionary<string, object>? Metadata { get; set; }
    }
};