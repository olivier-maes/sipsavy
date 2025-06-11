namespace SipSavy.Worker.Features.Video.AddNewVideos;

internal sealed record AddNewVideosResponse
{
    public List<VideoDto> Videos { get; init; } = [];

    internal sealed record VideoDto
    {
        public int Id { get; init; }
        public string YoutubeVideoId { get; init; } = string.Empty;
        public string Title { get; init; } = string.Empty;
    }
}