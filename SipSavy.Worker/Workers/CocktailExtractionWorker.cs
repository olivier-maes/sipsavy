using Mediator;
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
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        // Get all videos that need cocktail extraction
        var getVideosByStatusResponse =
            await mediator.Send(new GetVideosByStatusRequest(Status.TranscriptionFetched),
                cancellationToken);

        foreach (var v in getVideosByStatusResponse.Videos)
        {
            // Extract cocktails from the video
            var extractCocktailsResponse =
                await mediator.Send(new ExtractCocktailsRequest(v.Id), cancellationToken);

            // Save cocktails to the database
            var addNewCocktailsResponse = await mediator.Send(new AddNewCocktailsRequest
            {
                VideoId = v.Id,
                Cocktails = extractCocktailsResponse.Cocktails.Select(x => new AddNewCocktailsRequest.CocktailDto
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
            }, cancellationToken);

            // Update the video status to CocktailExtracted
            if (addNewCocktailsResponse.Cocktails.Count > 0)
            {
                var updateVideoResponse = await mediator.Send(
                    new UpdateVideoRequest(v.Id, null, Status.Processed), cancellationToken);
            }
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