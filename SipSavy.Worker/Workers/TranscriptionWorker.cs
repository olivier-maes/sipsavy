using SipSavy.Worker.Data.Domain;
using SipSavy.Worker.Features.Video.AddNewVideos;
using SipSavy.Worker.Features.Video.GetVideosByStatus;
using SipSavy.Worker.Features.Video.UpdateVideo;
using SipSavy.Worker.Youtube.Features.ExtractTranscription;
using SipSavy.Worker.Youtube.Features.GetVideosByChannelId;

namespace SipSavy.Worker.Workers;

internal sealed class TranscriptionWorker(
    ILogger<TranscriptionWorker> logger,
    IServiceScopeFactory serviceScopeFactory)
    : IHostedService, IDisposable
{
    private Timer? _timer;

    public Task StartAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Transcription worker running at: {Time}", DateTimeOffset.Now);
        _timer = new Timer(async void (_) => await DoWork(), null, TimeSpan.Zero, TimeSpan.FromHours(1));
        return Task.CompletedTask;
    }

    private async Task DoWork()
    {
        using var scope = serviceScopeFactory.CreateScope();
        var getVideosByChannelIdHandler = scope.ServiceProvider.GetRequiredService<GetVideosByChannelIdHandler>();
        var extractTranscriptionHandler = scope.ServiceProvider.GetRequiredService<ExtractTranscriptionHandler>();
        var addNewVideosHandler = scope.ServiceProvider.GetRequiredService<AddNewVideosHandler>();
        var getVideosByStatusHandler = scope.ServiceProvider.GetRequiredService<GetVideosByStatusHandler>();
        var updateVideoHandler = scope.ServiceProvider.GetRequiredService<UpdateVideoHandler>();

        // Get all videos from a specific YouTube channel
        var videosByChannelIdResponse = await getVideosByChannelIdHandler
            .Handle(new GetVideosByChannelIdRequest("UCioZY1p0bZ4Xt-yodw8_cBQ"));

        // Add the new videos to the database
        await addNewVideosHandler.Handle(new AddNewVideosRequest
        {
            Videos = videosByChannelIdResponse.Videos.Select(x => new AddNewVideosRequest.VideoDto
            {
                VideoId = x.Id,
                Title = x.Title
            }).ToList()
        });

        // Get videos that need transcription
        var getVideosByStatusResponse = await getVideosByStatusHandler.Handle(new GetVideosByStatusRequest(Status.New));
        foreach (var v in getVideosByStatusResponse.Videos)
        {
            // Extract transcription
            var extractTranscriptionResponse =
                await extractTranscriptionHandler.Handle(new ExtractTranscriptionRequest(v.YoutubeId));

            // Update video
            var updatedVideo = await updateVideoHandler.Handle(new UpdateVideoRequest(v.Id,
                extractTranscriptionResponse.Transcription, Status.TranscriptionFetched));
        }
    }

    public Task StopAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Transcription worker stopping");
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}