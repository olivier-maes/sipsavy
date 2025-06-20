using System.Text.Json.Serialization;
using SipSavy.Data.Domain;

namespace SipSavy.Worker.Features.Cocktail.ExtractCocktails;

public record ExtractCocktailsResponse
{
    public List<CocktailDto> Cocktails { get; set; } = [];

    public record CocktailDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<IngredientDto> Ingredients { get; set; } = [];
    }

    public record IngredientDto
    {
        public string Name { get; set; } = string.Empty;
        public float Quantity { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Unit Unit { get; set; }
    }
};