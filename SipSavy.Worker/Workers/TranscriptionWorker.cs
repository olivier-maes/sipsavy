using Mediator;
using SipSavy.Data.Domain;
using SipSavy.Worker.Features.Video.AddNewVideos;
using SipSavy.Worker.Features.Video.GetVideosByStatus;
using SipSavy.Worker.Features.Video.UpdateVideo;
using SipSavy.Worker.Features.Youtube.ExtractTranscription;
using SipSavy.Worker.Features.Youtube.GetVideosByChannelId;

namespace SipSavy.Worker.Workers;

internal sealed class TranscriptionWorker(
    ILogger<TranscriptionWorker> logger,
    IServiceScopeFactory serviceScopeFactory) : BackgroundService
{
    private const string Name = nameof(TranscriptionWorker);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("{Name} is running.", Name);
        while (!stoppingToken.IsCancellationRequested)
        {
            using IServiceScope scope = serviceScopeFactory.CreateScope();

            IScopedProcessingService scopedProcessingService =
                scope.ServiceProvider.GetRequiredService<IScopedProcessingService>();

            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            await scopedProcessingService.DoWorkAsync(stoppingToken);

            await DoWork(mediator, stoppingToken);
            await Task.Delay(10_000, stoppingToken);
        }
    }

    private async Task DoWork(IMediator mediator, CancellationToken cancellationToken)
    {
        var youtubeChannelId = Environment.GetEnvironmentVariable("YOUTUBE_CHANNEL_ID") ??
                               throw new Exception("YOUTUBE_CHANNEL_ID environment variable not set");

        // Get all videos from a specific YouTube channel
        var videosByChannelIdResponse =
            await mediator.Send(new GetVideosByChannelIdRequest(youtubeChannelId), cancellationToken);

        // Add the new videos to the database
        await mediator.Send(new AddNewVideosRequest
        {
            Videos = videosByChannelIdResponse.Videos.Select(x => new AddNewVideosRequest.VideoDto
            {
                VideoId = x.Id,
                Title = x.Title
            }).ToList()
        }, cancellationToken);

        // Get videos that need transcription
        var getVideosByStatusResponse =
            await mediator.Send(new GetVideosByStatusRequest(Status.New), cancellationToken);
        foreach (var v in getVideosByStatusResponse.Videos)
        {
            // Extract transcription
            var extractTranscriptionResponse = await mediator.Send(
                new ExtractTranscriptionRequest(v.YoutubeId),
                cancellationToken
            );

            // Update video
            await mediator.Send(new UpdateVideoRequest(
                v.Id,
                extractTranscriptionResponse.Transcription,
                Status.TranscriptionFetched
            ), cancellationToken);
        }
    }
}