using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using OllamaSharp;
using OllamaSharp.Models;
using SipSavy.Core;
using SipSavy.Data;
using SipSavy.Data.Domain;
using SipSavy.Worker.Features.Chunk.GetContextChunks;

namespace SipSavy.Worker.Features.Cocktail.ExtractCocktails;

internal sealed class ExtractCocktailsHandler
    : IHandler<ExtractCocktailsRequest, ExtractCocktailsResponse>
{
    private readonly IQueryFacade _queryFacade;
    private readonly IOllamaApiClient _ollamaApiClient;
    private readonly GetContextChunksHandler _getContextChunksHandler;
    private readonly string _model;

    public ExtractCocktailsHandler(IQueryFacade queryFacade, IOllamaApiClient ollamaApiClient,
        GetContextChunksHandler getContextChunksHandler)
    {
        _queryFacade = queryFacade;
        _ollamaApiClient = ollamaApiClient;
        _getContextChunksHandler = getContextChunksHandler;
        _model = Environment.GetEnvironmentVariable("AI_CHAT_MODEL") ??
                 throw new Exception("AI_CHAT_MODEL environment variable is not set.");
        _ollamaApiClient.SelectedModel = _model;
    }

    public async Task<ExtractCocktailsResponse> Handle(
        ExtractCocktailsRequest request,
        CancellationToken cancellationToken
    )
    {
        var fullResponse = new StringBuilder();
        var video = await _queryFacade.Videos.SingleAsync(x => x.Id == request.VideoId, cancellationToken);

        var getContextChunksResponse =
            await _getContextChunksHandler.Handle(new GetContextChunksRequest(video.Transcription), cancellationToken);

        var prompt = BuildRagPrompt(video.Transcription, getContextChunksResponse.Context);

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

    private static string BuildRagPrompt(string transcript, string context = "")
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
                Extract a cocktail recipe from the following transcript. Use the similar recipes below as context to fill in missing details and correct any errors.

                TRANSCRIPT:
                {transcript}

                SIMILAR RECIPES FOR CONTEXT:
                {context}

                Extract the recipe and return it as JSON with this structure:
                {format}

                The possible units are: {string.Join(", ", Enum.GetNames<Unit>())}

                Be precise with measurements and use the context recipes to infer missing information.
                Only return the JSON object without any additional text or explanation.
                """;
    }
}