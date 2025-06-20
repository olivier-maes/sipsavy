using SipSavy.Data.Domain;

namespace SipSavy.Worker.Features.Video.GetVideosByStatus;

internal sealed record GetVideosByStatusResponse
{
    public required List<VideoDto> Videos { get; init; }

    public record VideoDto
    {
        public int Id { get; init; }
        public string YoutubeId { get; init; } = string.Empty;
        public string Title { get; init; } = string.Empty;
        public string Transcription { get; set; } = string.Empty;
        public Status Status { get; init; }
    }
};