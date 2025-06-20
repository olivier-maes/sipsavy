namespace SipSavy.Data.Domain;

public sealed class Cocktail
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<CocktailIngredient> Ingredients { get; set; } = [];

    public int VideoId { get; set; }
    public Video Video { get; set; } = null!;
}