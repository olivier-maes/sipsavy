using Mediator;
using SipSavy.Data.Domain;
using SipSavy.Worker.Features.Video.AddNewVideos;
using SipSavy.Worker.Features.Video.GetVideosByStatus;
using SipSavy.Worker.Features.Video.UpdateVideo;
using SipSavy.Worker.Features.Youtube.ExtractTranscription;
using SipSavy.Worker.Features.Youtube.GetVideosByChannelId;

namespace SipSavy.Worker.Workers;

internal sealed class TranscriptionWorker(IServiceScopeFactory serviceScopeFactory) : IHostedService, IDisposable
{
    private Timer? _timer;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine($"Transcription worker running at: {DateTimeOffset.Now}");
        _timer = new Timer(async void (_) => await DoWork(cancellationToken), null, TimeSpan.Zero,
            TimeSpan.FromHours(1));
        return Task.CompletedTask;
    }

    private async Task DoWork(CancellationToken cancellationToken)
    {
        using var scope = serviceScopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var youtubeChannelId = Environment.GetEnvironmentVariable("YOUTUBE_CHANNEL_ID") ??
                               throw new Exception("YOUTUBE_CHANNEL_ID environment variable not set");

        // Get all videos from a specific YouTube channel
        var videosByChannelIdResponse = await mediator.Send(new GetVideosByChannelIdRequest(youtubeChannelId), cancellationToken);

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

    public Task StopAsync(CancellationToken stoppingToken)
    {
        Console.WriteLine("Transcription worker stopping");
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}