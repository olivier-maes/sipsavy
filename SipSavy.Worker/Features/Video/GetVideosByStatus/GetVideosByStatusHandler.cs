using Microsoft.EntityFrameworkCore;
using SipSavy.Core;
using SipSavy.Data;

namespace SipSavy.Worker.Features.Video.GetVideosByStatus;

internal sealed class GetVideosByStatusHandler(IQueryFacade queryFacade)
    : IHandler<GetVideosByStatusRequest, GetVideosByStatusResponse>
{
    public async Task<GetVideosByStatusResponse> Handle(GetVideosByStatusRequest request,
        CancellationToken cancellationToken)
    {
        var videos = queryFacade.Videos.Where(x => x.Status == request.Status);

        return new GetVideosByStatusResponse
        {
            Videos = await videos.Select(x => new GetVideosByStatusResponse.VideoDto
            {
                Id = x.Id,
                YoutubeId = x.YoutubeId,
                Title = x.Title,
                Transcription = x.Transcription,
                Status = x.Status
            }).ToListAsync(cancellationToken)
        };
    }
}