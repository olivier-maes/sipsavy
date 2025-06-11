using SipSavy.Core;
using YoutubeExplode;
using YoutubeExplode.Common;

namespace SipSavy.Worker.Youtube.Features.GetVideosByChannelId;

public sealed class GetVideosByChannelIdHandler(YoutubeClient youtubeClient)
    : IHandler<GetVideosByChannelIdRequest, GetVideosByChannelIdResponse>
{
    public async Task<GetVideosByChannelIdResponse> Handle(GetVideosByChannelIdRequest request)
    {
        var videos = await youtubeClient.Channels.GetUploadsAsync(request.ChannelId);

        return new GetVideosByChannelIdResponse
        {
            Videos = videos.Select(x => new GetVideosByChannelIdResponse.VideoDto
            {
                Id = x.Id,
                Title = x.Title,
            }).ToList()
        };
    }
}