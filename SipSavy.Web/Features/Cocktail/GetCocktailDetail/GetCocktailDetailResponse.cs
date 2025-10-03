using SipSavy.Data.Domain;

namespace SipSavy.Web.Features.Cocktail.GetCocktailDetail;

internal sealed record GetCocktailDetailResponse
{
    public CocktailDto Cocktail { get; set; } = new();

    internal sealed record CocktailDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public List<IngredientDto> Ingredients { get; set; } = [];

        public int VideoId { get; set; }
    }

    internal sealed record IngredientDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public float Quantity { get; set; }
        public Unit Unit { get; set; }
    }
};