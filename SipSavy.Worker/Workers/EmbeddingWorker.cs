using SipSavy.Worker.AI.Features.Chunk.ChunkTextBySentence;
using SipSavy.Worker.Data.Domain;
using SipSavy.Worker.Features.Video.GetVideosByStatus;

namespace SipSavy.Worker.Workers;

internal sealed class EmbeddingWorker(
    ILogger<EmbeddingWorker> logger,
    IServiceScopeFactory serviceScopeFactory)
    : IHostedService, IDisposable
{
    private Timer? _timer;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Embedding worker running at: {Time}", DateTimeOffset.Now);
        _timer = new Timer(async void (_) => await DoWork(), null, TimeSpan.Zero, TimeSpan.FromHours(1));
        return Task.CompletedTask;
    }

    private async Task DoWork()
    {
        using var scope = serviceScopeFactory.CreateScope();
        var getVideosByStatusHandler = scope.ServiceProvider.GetRequiredService<GetVideosByStatusHandler>();
        var chunkTextBySentenceHandler =
            scope.ServiceProvider.GetRequiredService<ChunkTextBySentenceHandler>();

        // Get all videos that need embedding
        var getVideosByStatusResponse =
            await getVideosByStatusHandler.Handle(new GetVideosByStatusRequest(Status.TranscriptionFetched));

        foreach (var v in getVideosByStatusResponse.Videos)
        {
            // Chunk the transcription
            var chunkTextBySentenceResponse =
                await chunkTextBySentenceHandler.Handle(new ChunkTextBySentenceRequest(v.Transcription));
            
            
            // Embed the transcription chunks
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Embedding worker stopping");
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}