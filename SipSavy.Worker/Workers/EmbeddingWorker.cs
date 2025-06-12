using SipSavy.Worker.AI.Features.Chunk.ChunkTextByFixedSize;
using SipSavy.Worker.AI.Features.Chunk.ChunkTextBySentence;
using SipSavy.Worker.AI.Features.Embedding.GetEmbeddings;
using SipSavy.Worker.Data.Domain;
using SipSavy.Worker.Features.Video.GetVideosByStatus;
using SipSavy.Worker.Features.Video.UpdateVideo;

namespace SipSavy.Worker.Workers;

internal sealed class EmbeddingWorker(IServiceScopeFactory serviceScopeFactory) : IHostedService, IDisposable
{
    private Timer? _timer;

    public Task StartAsync(CancellationToken cancellationToken)
    {
       Console.WriteLine($"Embedding worker running at: {DateTimeOffset.Now}");
        _timer = new Timer(async void (_) => await DoWork(), null, TimeSpan.Zero, TimeSpan.FromHours(1));
        return Task.CompletedTask;
    }

    private async Task DoWork()
    {
        using var scope = serviceScopeFactory.CreateScope();
        var getVideosByStatusHandler = scope.ServiceProvider.GetRequiredService<GetVideosByStatusHandler>();
        var updateVideoHandler = scope.ServiceProvider.GetRequiredService<UpdateVideoHandler>();
        var getEmbeddingsHandler = scope.ServiceProvider.GetRequiredService<GetEmbeddingsHandler>();
        var chunkTextByFixedSizeHandler =
            scope.ServiceProvider.GetRequiredService<ChunkTextByFixedSizeHandler>();

        // Get all videos that need embedding
        var getVideosByStatusResponse =
            await getVideosByStatusHandler.Handle(new GetVideosByStatusRequest(Status.TranscriptionFetched));

        foreach (var v in getVideosByStatusResponse.Videos)
        {
            // Chunk the transcription
            var chunkTextByFixedSizeResponse =
                await chunkTextByFixedSizeHandler.Handle(new ChunkTextByFixedSizeRequest(v.Transcription));

            // Embed the transcription chunks
            foreach (var chunk in chunkTextByFixedSizeResponse.Chunks)
            {
                var getEmbeddingsHandlerResponse = await getEmbeddingsHandler.Handle(
                    new GetEmbeddingsRequest(chunk.Content));
            }

            // Update the video status to Embedded
            var updateVideoResponse = await updateVideoHandler.Handle(
                new UpdateVideoRequest(v.Id, v.Transcription, Status.Embedded));
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("Embedding worker stopping");
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}