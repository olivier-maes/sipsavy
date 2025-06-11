using System.Text;
using System.Text.RegularExpressions;
using SipSavy.Core;

namespace SipSavy.Worker.AI.Features.Chunk.ChunkTextBySentence;

public sealed class ChunkTextBySentenceHandler : IHandler<ChunkTextBySentenceRequest, ChunkTextBySentenceResponse>
{
    public async Task<ChunkTextBySentenceResponse> Handle(ChunkTextBySentenceRequest request)
    {
        var chunks = new List<ChunkTextBySentenceResponse.TextChunkDto>();
        var sentences = SplitIntoSentences(request.Text);
        var currentChunk = new StringBuilder();
        var chunkIndex = 0;
        var sentenceBuffer = new List<string>();

        foreach (var sentence in sentences)
        {
            var trimmedSentence = sentence.Trim();
            if (string.IsNullOrEmpty(trimmedSentence)) continue;

            if (currentChunk.Length + trimmedSentence.Length + 1 > request.MaxChunkSize && currentChunk.Length > 0)
            {
                chunks.Add(new ChunkTextBySentenceResponse.TextChunkDto
                {
                    Content = currentChunk.ToString().Trim(),
                    Index = chunkIndex++,
                    ChunkingMethod = "Sentence"
                });

                currentChunk.Clear();
                var overlapText = GetOverlapText(sentenceBuffer, request.Overlap);
                if (!string.IsNullOrEmpty(overlapText))
                {
                    currentChunk.Append(overlapText).Append(' ');
                }

                sentenceBuffer.Clear();
            }

            if (currentChunk.Length > 0) currentChunk.Append(' ');
            currentChunk.Append(trimmedSentence);
            sentenceBuffer.Add(trimmedSentence);
        }

        if (currentChunk.Length > 0)
        {
            chunks.Add(new ChunkTextBySentenceResponse.TextChunkDto
            {
                Content = currentChunk.ToString().Trim(),
                Index = chunkIndex,
                ChunkingMethod = "Sentence"
            });
        }

        return new ChunkTextBySentenceResponse
        {
            Chunks = chunks
        };
    }

    private static List<string> SplitIntoSentences(string text)
    {
        const string sentencePattern = @"(?<=[.!?])\s+(?=[A-Z])";
        var sentences = Regex.Split(text, sentencePattern, RegexOptions.Multiline)
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .ToList();

        // Handle edge cases where regex might not work well
        if (sentences.Count <= 1)
        {
            // Fall back to simple splitting
            sentences = text.Split(['.', '!', '?'], StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim())
                .Where(s => !string.IsNullOrEmpty(s))
                .Select(s => s + (text.Contains(s + ".") ? "." :
                    text.Contains(s + "!") ? "!" :
                    text.Contains(s + "?") ? "?" : ""))
                .ToList();
        }

        return sentences;
    }

    private static string GetOverlapText(List<string> buffer, int maxOverlapLength)
    {
        if (buffer.Count == 0) return string.Empty;

        var overlap = new StringBuilder();
        for (var i = buffer.Count - 1; i >= 0; i--)
        {
            var candidate = buffer[i] + " " + overlap.ToString();
            if (candidate.Length <= maxOverlapLength)
            {
                overlap.Insert(0, buffer[i] + " ");
            }
            else
            {
                break;
            }
        }

        return overlap.ToString().Trim();
    }
}