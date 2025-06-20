using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using OllamaSharp;
using OllamaSharp.Models;
using SipSavy.Core;
using SipSavy.Worker.Data;
using SipSavy.Worker.Data.Domain;

namespace SipSavy.Worker.AI.Features.Cocktail.ExtractCocktails;

public class ExtractCocktailsHandler
    : IHandler<ExtractCocktailsRequest, ExtractCocktailsResponse>
{
    private readonly IQueryFacade _queryFacade;
    private readonly IOllamaApiClient _ollamaApiClient;

    public ExtractCocktailsHandler(IQueryFacade queryFacade, IOllamaApiClient ollamaApiClient)
    {
        _queryFacade = queryFacade;
        _ollamaApiClient = ollamaApiClient;
        _ollamaApiClient.SelectedModel = "llama3.1";
    }

    public async Task<ExtractCocktailsResponse> Handle(
        ExtractCocktailsRequest request,
        CancellationToken cancellationToken
    )
    {
        var fullResponse = new StringBuilder();
        var video = await _queryFacade.Videos.SingleAsync(x => x.Id == request.VideoId, cancellationToken);
        var prompt = BuildRagPrompt(video.Transcription);

        var ollamaRequest = new GenerateRequest
        {
            Model = "llama3.1",
            Prompt = prompt,
            Stream = true,
            Options = new RequestOptions
            {
                Temperature = 0.1f,
                TopK = 10,
                TopP = 0.9f,
                NumPredict = 500
            }
        };

        await foreach (var response in _ollamaApiClient.GenerateAsync(ollamaRequest, cancellationToken))
        {
            fullResponse.Append(response?.Response);

            if (response is not null && response.Done)
                break;
        }

        var jsonResponse = fullResponse.ToString();
        Console.WriteLine(jsonResponse);

        return new ExtractCocktailsResponse();
    }

    private static string BuildRagPrompt(string transcript)
    {
        var format = JsonSerializer.Serialize(new ExtractCocktailsResponse.CocktailDto
        {
            Name = "Recipe name",
            Description = "Recipe description",
            Ingredients =
            [
                new ExtractCocktailsResponse.IngredientDto
                {
                    Name = "Ingredient name",
                    Quantity = 0.5f,
                    Unit = Unit.Ounce
                }
            ]
        });

        return $"""
                Extract a cocktail recipe from the following transcript. Use the similar recipes below as context to fill in missing details and correct any errors.

                TRANSCRIPT:
                {transcript}

                SIMILAR RECIPES FOR CONTEXT:

                Extract the recipe and return it as JSON with this structure:
                {format}

                Be precise with measurements and use the context recipes to infer missing information.
                """;
    }
}