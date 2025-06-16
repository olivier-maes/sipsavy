namespace SipSavy.Worker.AI.Features.Chunk.ChunkTextBySentence;

public record ChunkTextBySentenceRequest(string Text, int MaxChunkSize = 1000, int Overlap = 100);