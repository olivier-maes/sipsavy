namespace SipSavy.Web.Features.Cocktail.GetCocktailsOverview;

internal sealed record GetCocktailsOverviewResponse
{
    public List<CocktailDto> Cocktails { get; init; } = [];

    internal sealed record CocktailDto
    {
        public int Id { get; set; }
        public string Name { get; init; } = string.Empty;
    }
};