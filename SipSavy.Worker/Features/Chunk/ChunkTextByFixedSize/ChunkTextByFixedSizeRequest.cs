namespace SipSavy.Worker.Features.Chunk.ChunkTextByFixedSize;

public sealed record ChunkTextByFixedSizeRequest(string Text, int ChunkSize = 1000, int Overlap = 200);