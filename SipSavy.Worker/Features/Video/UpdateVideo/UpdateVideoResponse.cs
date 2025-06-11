using SipSavy.Worker.Data.Domain;

namespace SipSavy.Worker.Features.Video.UpdateVideo;

internal sealed record UpdateVideoResponse
{
    public VideoDto? Video { get; set; }

    internal sealed record VideoDto
    {
        public int Id { get; set; }
        public string YoutubeId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Transcription { get; set; } = string.Empty;
        public Status Status { get; set; }
    }
};