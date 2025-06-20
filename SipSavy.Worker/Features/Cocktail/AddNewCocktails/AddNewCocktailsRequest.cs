using SipSavy.Data.Domain;

namespace SipSavy.Worker.Features.Cocktail.AddNewCocktails;

internal sealed record AddNewCocktailsRequest
{
    public int VideoId { get; set; }
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
        public Unit Unit { get; set; }
    }
};