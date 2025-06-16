using SipSavy.Worker.AI.Features.Cocktail.ExtractCocktails;
using SipSavy.Worker.Data.Domain;
using SipSavy.Worker.Features.Video.GetVideosByStatus;
using SipSavy.Worker.Features.Video.UpdateVideo;

namespace SipSavy.Worker.Workers;

internal sealed class ExtractCocktailWorker(IServiceScopeFactory serviceScopeFactory) : IHostedService, IDisposable
{
    private Timer? _timer;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine($"Cocktail extraction worker running at: {DateTimeOffset.Now}");
        _timer = new Timer(async void (_) => await DoWork(), null, TimeSpan.Zero, TimeSpan.FromHours(1));
        return Task.CompletedTask;
    }

    private async Task DoWork()
    {
        using var scope = serviceScopeFactory.CreateScope();
        var getVideosByStatusHandler = scope.ServiceProvider.GetRequiredService<GetVideosByStatusHandler>();
        var updateVideoHandler = scope.ServiceProvider.GetRequiredService<UpdateVideoHandler>();
        var extractCocktailsHandler = scope.ServiceProvider.GetRequiredService<ExtractCocktailsHandler>();

        // Get all videos that need cocktail extraction
        var getVideosByStatusResponse =
            await getVideosByStatusHandler.Handle(new GetVideosByStatusRequest(Status.Embedded));

        foreach (var v in getVideosByStatusResponse.Videos)
        {
            // Extract cocktails from the video
            var extractCocktailsResponse =
                await extractCocktailsHandler.Handle(new ExtractCocktailsRequest(v.Id));

            // TODO: Send cocktails to SipSavy.Web

            // Update the video status to CocktailExtracted
            var updateVideoResponse = await updateVideoHandler.Handle(
                new UpdateVideoRequest(v.Id, null, Status.Processed));
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("Cocktail extraction stopping");
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}