using SipSavy.Worker.Data.Domain;

namespace SipSavy.Worker.AI.Features.Cocktail.ExtractCocktails;

public record ExtractCocktailsResponse
{
    public List<CocktailDto> Cocktail { get; set; } = [];

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
        public Unit Unit { get; set; }
    }
};