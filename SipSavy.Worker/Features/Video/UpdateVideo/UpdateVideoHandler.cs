using Mediator;
using SipSavy.Data.Repository;

namespace SipSavy.Worker.Features.Video.UpdateVideo;

internal sealed class UpdateVideoHandler(IVideoRepository videoRepository)
    : IRequestHandler<UpdateVideoRequest, UpdateVideoResponse>
{
    public async ValueTask<UpdateVideoResponse> Handle(UpdateVideoRequest request, CancellationToken cancellationToken)
    {
        var video = await videoRepository.UpdateVideo(request.Id, request.Transcription, request.Status);
        if (video is null)
        {
            return new UpdateVideoResponse();
        }

        return new UpdateVideoResponse
        {
            Video = new UpdateVideoResponse.VideoDto
            {
                Id = video.Id,
                YoutubeId = video.YoutubeId,
                Title = video.Title,
                Transcription = video.Transcription,
                Status = video.Status,
            }
        };
    }
}