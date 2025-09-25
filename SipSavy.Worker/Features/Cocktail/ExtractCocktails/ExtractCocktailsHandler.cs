using System.Text;
using System.Text.Json;
using Mediator;
using Microsoft.EntityFrameworkCore;
using OllamaSharp;
using OllamaSharp.Models;
using SipSavy.Data;
using Unit = SipSavy.Data.Domain.Unit;

namespace SipSavy.Worker.Features.Cocktail.ExtractCocktails;

internal sealed class ExtractCocktailsHandler
    : IRequestHandler<ExtractCocktailsRequest, ExtractCocktailsResponse>
{
    private readonly IQueryFacade _queryFacade;
    private readonly IOllamaApiClient _ollamaApiClient;
    private readonly string _model;

    public ExtractCocktailsHandler(IQueryFacade queryFacade, IOllamaApiClient ollamaApiClient)
    {
        _queryFacade = queryFacade;
        _ollamaApiClient = ollamaApiClient;
        _model = Environment.GetEnvironmentVariable("AI_CHAT_MODEL") ??
                 throw new Exception("AI_CHAT_MODEL environment variable is not set.");
        _ollamaApiClient.SelectedModel = _model;
    }

    public async ValueTask<ExtractCocktailsResponse> Handle(ExtractCocktailsRequest request,
        CancellationToken cancellationToken)
    {
        var fullResponse = new StringBuilder();
        var video = await _queryFacade.Videos.SingleAsync(x => x.Id == request.VideoId, cancellationToken);

        var prompt = BuildPrompt(video.Transcription);

        var ollamaRequest = new GenerateRequest
        {
            Model = _model,
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

        await foreach (var res in _ollamaApiClient.GenerateAsync(ollamaRequest, cancellationToken))
        {
            fullResponse.Append(res?.Response);

            if (res is not null && res.Done)
                break;
        }

        var jsonResponse = fullResponse.ToString();

        try
        {
            return JsonSerializer.Deserialize<ExtractCocktailsResponse>(jsonResponse) ?? new ExtractCocktailsResponse();
        }
        catch (JsonException e)
        {
            Console.WriteLine($"Failed to deserialize response: {e.Message}");
            Console.WriteLine($"Response: {jsonResponse}");
            return new ExtractCocktailsResponse();
        }
    }

    private static string BuildPrompt(string transcript)
    {
        var format = JsonSerializer.Serialize(new ExtractCocktailsResponse
        {
            Cocktails =
            [
                new ExtractCocktailsResponse.CocktailDto
                {
                    Name = "Recipe name",
                    Description = "Recipe description",
                    Ingredients =
                    [
                        new ExtractCocktailsResponse.IngredientDto
                        {
                            Name = "Ingredient name",
                            Quantity = 0.5f,
                            Unit = Unit.Ounces
                        }
                    ]
                }
            ]
        });

        return $"""
                Extract one or more cocktail recipe(s) from the following transcript.

                TRANSCRIPT:
                {transcript}

                Extract the recipe and return it as JSON with this structure:
                {format}

                The possible units are: {string.Join(", ", Enum.GetNames<Unit>())}

                Be precise with measurements.
                Only return the JSON object without any additional text or explanation.
                """;
    }
}