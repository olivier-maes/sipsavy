using System.Text.Json;
using OllamaSharp;
using OllamaSharp.Models.Chat;
using SipSavy.Core;
using SipSavy.Worker.Data.Domain;

namespace SipSavy.Worker.AI.Features.Cocktail.ExtractCocktails;

public class ExtractCocktailsHandler(IOllamaApiClient ollamaApiClient)
    : IHandler<ExtractCocktailsRequest, ExtractCocktailsResponse>
{
    private readonly string _basePrompt =
        "You are an expert bartender. Your task is to extract cocktail recipes from a video transcript. " +
        "The transcript will be provided to you, and you need to identify the cocktails mentioned, " +
        "their ingredients, and any specific instructions given in the video. " +
        "Please provide the extracted cocktails in a structured format.";

    public async Task<ExtractCocktailsResponse> Handle(ExtractCocktailsRequest request)
    {
        var example = new ExtractCocktailsResponse.CocktailDto
        {
            Name = "The cocktail name",
            Description = "A brief description of the cocktail",
            Ingredients = new List<ExtractCocktailsResponse.IngredientDto>(
            [
                new ExtractCocktailsResponse.IngredientDto
                {
                    Name = "Ingredient name",
                    Quantity = 1.0f,
                    Unit = Unit.Ounce
                }
            ])
        };

        var format = JsonSerializer.Serialize(example);
        var prompt = $"{_basePrompt}\n\nExample:\n{format}";
        var chat = new Chat(ollamaApiClient);
        chat.Messages.Add(new Message(ChatRole.System, prompt));

        var response =  chat.SendAsync(prompt);

        return new ExtractCocktailsResponse();
    }
}