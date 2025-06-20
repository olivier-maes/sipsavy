using SipSavy.Data.Domain;
using SipSavy.Worker.Features.Cocktail.AddNewCocktails;
using SipSavy.Worker.Features.Cocktail.ExtractCocktails;
using SipSavy.Worker.Features.Video.GetVideosByStatus;
using SipSavy.Worker.Features.Video.UpdateVideo;

namespace SipSavy.Worker.Workers;

internal sealed class CocktailExtractionWorker(IServiceScopeFactory serviceScopeFactory) : IHostedService, IDisposable
{
    private Timer? _timer;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine($"Cocktail extraction worker running at: {DateTimeOffset.Now}");
        _timer = new Timer(async void (_) => await DoWork(cancellationToken), null, TimeSpan.Zero,
            TimeSpan.FromHours(1));
        return Task.CompletedTask;
    }

    private async Task DoWork(CancellationToken cancellationToken)
    {
        using var scope = serviceScopeFactory.CreateScope();
        var getVideosByStatusHandler = scope.ServiceProvider.GetRequiredService<GetVideosByStatusHandler>();
        var updateVideoHandler = scope.ServiceProvider.GetRequiredService<UpdateVideoHandler>();
        var extractCocktailsHandler = scope.ServiceProvider.GetRequiredService<ExtractCocktailsHandler>();
        var addNewCocktailsHandler = scope.ServiceProvider.GetRequiredService<AddNewCocktailsHandler>();

        // Get all videos that need cocktail extraction
        var getVideosByStatusResponse =
            await getVideosByStatusHandler.Handle(new GetVideosByStatusRequest(Status.Embedded), cancellationToken);

        foreach (var v in getVideosByStatusResponse.Videos)
        {
            // Extract cocktails from the video
            var extractCocktailsResponse =
                await extractCocktailsHandler.Handle(new ExtractCocktailsRequest(v.Id), cancellationToken);

            // Save cocktails to the database
            var addNewCocktailsResponse =
                await addNewCocktailsHandler.Handle(new AddNewCocktailsRequest
                    {
                        VideoId = v.Id,
                        Cocktails = extractCocktailsResponse.Cocktails.Select(x =>
                            new AddNewCocktailsRequest.CocktailDto
                            {
                                Name = x.Name,
                                Description = x.Description,
                                Ingredients = x.Ingredients.Select(i => new AddNewCocktailsRequest.IngredientDto
                                {
                                    Name = i.Name,
                                    Quantity = i.Quantity,
                                    Unit = i.Unit
                                }).ToList()
                            }).ToList()
                    },
                    cancellationToken);

            // Update the video status to CocktailExtracted
            var updateVideoResponse = await updateVideoHandler.Handle(
                new UpdateVideoRequest(v.Id, null, Status.Processed), cancellationToken);
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