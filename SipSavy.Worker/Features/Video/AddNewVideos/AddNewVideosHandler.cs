using Microsoft.EntityFrameworkCore;
using SipSavy.Core;
using SipSavy.Data;
using SipSavy.Data.Repository;

namespace SipSavy.Worker.Features.Video.AddNewVideos;

internal sealed class AddNewVideosHandler(IQueryFacade queryFacade, IVideoRepository videoRepository)
    : IHandler<AddNewVideosRequest, AddNewVideosResponse>
{
    public async Task<AddNewVideosResponse> Handle(AddNewVideosRequest request, CancellationToken cancellationToken)
    {
        var addedVideos = new List<Data.Domain.Video>();

        foreach (var video in request.Videos)
        {
            var existingVideo =
                await queryFacade.Videos.FirstOrDefaultAsync(x => x.YoutubeId == video.VideoId, cancellationToken);
            if (existingVideo is not null) continue;

            var newVideo = await videoRepository.AddVideo(new Data.Domain.Video
            {
                YoutubeId = video.VideoId,
                Title = video.Title
            });
            addedVideos.Add(newVideo);
        }

        return new AddNewVideosResponse
        {
            Videos = addedVideos.Select(x => new AddNewVideosResponse.VideoDto
            {
                Id = x.Id,
                YoutubeVideoId = x.YoutubeId,
                Title = x.Title
            }).ToList()
        };
    }
}