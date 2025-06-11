namespace SipSavy.Worker.Features.Video.AddNewVideos;

internal sealed record AddNewVideosRequest
{
    public List<VideoDto> Videos { get; init; } = [];

    internal sealed class VideoDto
    {
        public string VideoId { get; init; } = string.Empty;
        public string Title { get; init; } = string.Empty;
    }
};