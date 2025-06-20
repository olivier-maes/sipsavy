using SipSavy.Data.Domain;
using SipSavy.Worker.Features.Chunk.ChunkTextByFixedSize;
using SipSavy.Worker.Features.Embedding.GetEmbeddings;
using SipSavy.Worker.Features.Video.GetVideosByStatus;
using SipSavy.Worker.Features.Video.UpdateVideo;
using SipSavy.Worker.Features.VideoChunk.AddVideoChunks;

namespace SipSavy.Worker.Workers;

internal sealed class EmbeddingWorker(IServiceScopeFactory serviceScopeFactory) : IHostedService, IDisposable
{
    private Timer? _timer;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine($"Embedding worker running at: {DateTimeOffset.Now}");
        _timer = new Timer(async void (_) => await DoWork(cancellationToken), null, TimeSpan.Zero,
            TimeSpan.FromHours(1));
        return Task.CompletedTask;
    }

    private async Task DoWork(CancellationToken cancellationToken)
    {
        using var scope = serviceScopeFactory.CreateScope();
        var getVideosByStatusHandler = scope.ServiceProvider.GetRequiredService<GetVideosByStatusHandler>();
        var updateVideoHandler = scope.ServiceProvider.GetRequiredService<UpdateVideoHandler>();
        var getEmbeddingsHandler = scope.ServiceProvider.GetRequiredService<GetEmbeddingsHandler>();
        var chunkTextByFixedSizeHandler =
            scope.ServiceProvider.GetRequiredService<ChunkTextByFixedSizeHandler>();
        var addVideoChunksHandler =
            scope.ServiceProvider.GetRequiredService<AddVideoChunksHandler>();

        // Get all videos that need embedding
        var getVideosByStatusResponse =
            await getVideosByStatusHandler.Handle(new GetVideosByStatusRequest(Status.TranscriptionFetched),
                cancellationToken);

        foreach (var v in getVideosByStatusResponse.Videos)
        {
            var videoChunks = new List<AddVideoChunksRequest.VideoChunkDto>();

            // Chunk the transcription
            var chunkTextByFixedSizeResponse =
                await chunkTextByFixedSizeHandler.Handle(new ChunkTextByFixedSizeRequest(v.Transcription), cancellationToken);

            // Embed the transcription chunks
            foreach (var chunk in chunkTextByFixedSizeResponse.Chunks)
            {
                var getEmbeddingsHandlerResponse = await getEmbeddingsHandler.Handle(
                    new GetEmbeddingsRequest(chunk.Content), cancellationToken);

                videoChunks.Add(new AddVideoChunksRequest.VideoChunkDto
                {
                    Content = chunk.Content,
                    Embedding = getEmbeddingsHandlerResponse.Embedding,
                    VideoId = v.Id
                });
            }

            // Save the embeddings to the vector store
            await addVideoChunksHandler.Handle(new AddVideoChunksRequest { VideoChunks = videoChunks },
                cancellationToken);

            // Update the video status to Embedded
            await updateVideoHandler.Handle(
                new UpdateVideoRequest(v.Id, null, Status.Embedded), cancellationToken);
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