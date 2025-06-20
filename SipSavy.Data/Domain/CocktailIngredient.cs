namespace SipSavy.Data.Domain;

public sealed class CocktailIngredient
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public float Quantity { get; set; }
    public Unit Unit { get; set; } = Unit.None;

    public int CocktailId { get; set; }
    public Cocktail Cocktail { get; set; } = null!;
}