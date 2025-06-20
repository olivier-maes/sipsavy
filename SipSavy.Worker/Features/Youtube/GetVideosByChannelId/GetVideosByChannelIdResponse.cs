namespace SipSavy.Worker.Features.Youtube.GetVideosByChannelId;

public record GetVideosByChannelIdResponse
{
    public List<VideoDto> Videos { get; init; } = [];

    public record VideoDto
    {
        public string Id { get; init; } = string.Empty;
        public string Title { get; init; } = string.Empty;
    }
};