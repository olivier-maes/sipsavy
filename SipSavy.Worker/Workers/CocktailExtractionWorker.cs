using Mediator;
using SipSavy.Data.Domain;
using SipSavy.Worker.Features.Cocktail.AddNewCocktails;
using SipSavy.Worker.Features.Cocktail.ExtractCocktails;
using SipSavy.Worker.Features.Video.GetVideosByStatus;
using SipSavy.Worker.Features.Video.UpdateVideo;

namespace SipSavy.Worker.Workers;

internal sealed class CocktailExtractionWorker(
    ILogger<CocktailExtractionWorker> logger,
    IServiceScopeFactory serviceScopeFactory)
    : BackgroundService
{
    private const string Name = nameof(CocktailExtractionWorker);

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
}