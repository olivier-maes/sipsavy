using Mediator;
using YoutubeExplode;
using YoutubeExplode.Common;

namespace SipSavy.Worker.Features.Youtube.GetVideosByChannelId;

public sealed class GetVideosByChannelIdHandler(YoutubeClient youtubeClient)
    : IRequestHandler<GetVideosByChannelIdRequest, GetVideosByChannelIdResponse>
{
    public async ValueTask<GetVideosByChannelIdResponse> Handle(GetVideosByChannelIdRequest request,
        CancellationToken cancellationToken)
    {
        var videos = await youtubeClient.Channels.GetUploadsAsync(request.ChannelId, cancellationToken);

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