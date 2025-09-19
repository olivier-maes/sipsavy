using Mediator;
using Unit = SipSavy.Data.Domain.Unit;

namespace SipSavy.Worker.Features.Cocktail.AddNewCocktails;

internal sealed record AddNewCocktailsRequest : IRequest<AddNewCocktailsResponse>
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